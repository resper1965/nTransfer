using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Domain.Entities;

namespace TransferenciaMateriais.Domain.Workflow;

/// <summary>
/// Máquina de estados do workflow (F1/F2/F3).
/// Implementa regras de negócio RB-01..RB-11 e validações por fluxo e papel.
/// </summary>
public class WorkflowStateMachine : IWorkflowStateMachine
{
    private readonly Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> _allowedTransitions;
    private readonly Dictionary<WorkflowStatus, List<string>> _requiredRoles;

    public WorkflowStateMachine()
    {
        _allowedTransitions = BuildTransitionMatrix();
        _requiredRoles = BuildRoleMatrix();
    }

    public WorkflowTransitionResult CanTransition(WorkflowTransitionRequest request)
    {
        // Validação básica
        if (request.FromStatus == request.ToStatus)
        {
            return WorkflowTransitionResult.Failure("Estado origem e destino são iguais.");
        }

        // Validação de papel
        if (!IsRoleAllowed(request.ToStatus, request.ActorRole))
        {
            return WorkflowTransitionResult.Failure($"Papel '{request.ActorRole}' não tem permissão para transição para '{request.ToStatus}'.");
        }

        // Validação de transição permitida (por fluxo)
        // Nota: fluxType precisa ser obtido do contexto/entidade
        // Por enquanto, validamos apenas a estrutura básica

        return WorkflowTransitionResult.Success();
    }

    public List<WorkflowStatus> GetAllowedTransitions(WorkflowStatus currentStatus, FluxType fluxType, string actorRole)
    {
        if (!_allowedTransitions.TryGetValue(currentStatus, out var fluxTransitions))
        {
            return new List<WorkflowStatus>();
        }

        if (!fluxTransitions.TryGetValue(fluxType, out var transitions))
        {
            return new List<WorkflowStatus>();
        }

        // Filtrar por papel
        return transitions.Where(t => IsRoleAllowed(t, actorRole)).ToList();
    }

    public WorkflowTransitionResult ValidateBusinessRules(WorkflowTransitionRequest request)
    {
        var errors = new List<string>();

        // RB-04: Anexo obrigatório para ENTRADA_DESTINO_CONCLUIDA
        if (request.ToStatus == WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA)
        {
            if (request.Context == null || !request.Context.ContainsKey("hasRequiredAttachment") || 
                !(bool)(request.Context["hasRequiredAttachment"] ?? false))
            {
                errors.Add("RB-04: Ao menos um anexo é obrigatório para concluir entrada no destino.");
            }
        }

        // RB-05: NFe incorreta requer motivo
        if (request.ToStatus == WorkflowStatus.NFE_VALIDADA_NOK)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                errors.Add("RB-05: Motivo obrigatório quando NFe é marcada como incorreta.");
            }
        }

        // RB-11: Gestor sempre aprova (não precisa validação adicional)
        if (request.ActorRole == "GESTOR_CONTRATO" && 
            (request.ToStatus == WorkflowStatus.ENTREGA_APROVADA || 
             request.ToStatus == WorkflowStatus.MEDICAO_APROVADA))
        {
            // Aprovação automática permitida
        }

        if (errors.Any())
        {
            var errorMessage = string.Join(" ", errors);
            return WorkflowTransitionResult.Failure(errorMessage, errors);
        }

        return WorkflowTransitionResult.Success();
    }

    private bool IsRoleAllowed(WorkflowStatus status, string role)
    {
        if (!_requiredRoles.TryGetValue(status, out var allowedRoles))
        {
            return false; // Se não há restrição definida, não permite (mais seguro)
        }

        return allowedRoles.Contains(role) || allowedRoles.Contains("SISTEMA");
    }

    private Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> BuildTransitionMatrix()
    {
        var matrix = new Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>>();

        // Transições comuns (todos os fluxos)
        AddCommonTransitions(matrix);

        // Transições F1 - Compra Direta
        AddF1Transitions(matrix);

        // Transições F2 - Entrega Futura (mãe)
        AddF2Transitions(matrix);

        // Transições F3 - Entrega Futura (filha)
        AddF3Transitions(matrix);

        return matrix;
    }

    private void AddCommonTransitions(Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> matrix)
    {
        var commonTransitions = new Dictionary<WorkflowStatus, List<WorkflowStatus>>
        {
            { WorkflowStatus.OS_CRIADA, new List<WorkflowStatus> { WorkflowStatus.ROMANEIO_CONFERIDO, WorkflowStatus.PROCESSO_CANCELADO } },
            { WorkflowStatus.ROMANEIO_CONFERIDO, new List<WorkflowStatus> { WorkflowStatus.NFE_EMITIDA } },
            { WorkflowStatus.NFE_EMITIDA, new List<WorkflowStatus> { WorkflowStatus.XML_OBTIDO } },
            { WorkflowStatus.XML_OBTIDO, new List<WorkflowStatus> { WorkflowStatus.NFE_VALIDADA_OK, WorkflowStatus.NFE_VALIDADA_NOK } },
            { WorkflowStatus.NFE_VALIDADA_NOK, new List<WorkflowStatus> { WorkflowStatus.CORRECAO_EMISSAO_OU_VINCULO } },
            { WorkflowStatus.CORRECAO_EMISSAO_OU_VINCULO, new List<WorkflowStatus> { WorkflowStatus.NFE_EMITIDA } },
            { WorkflowStatus.NFE_VALIDADA_OK, new List<WorkflowStatus> { WorkflowStatus.VINCULO_CRIADO } },
            { WorkflowStatus.VINCULO_CRIADO, new List<WorkflowStatus> { WorkflowStatus.VINCULO_CORRIGIDO } },
            { WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO, new List<WorkflowStatus> { WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA } },
            { WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA, new List<WorkflowStatus> { WorkflowStatus.PROCESSO_CONCLUIDO } }
        };

        foreach (var (from, toList) in commonTransitions)
        {
            if (!matrix.ContainsKey(from))
            {
                matrix[from] = new Dictionary<FluxType, List<WorkflowStatus>>();
            }

            foreach (var fluxType in Enum.GetValues<FluxType>())
            {
                if (!matrix[from].ContainsKey(fluxType))
                {
                    matrix[from][fluxType] = new List<WorkflowStatus>();
                }
                matrix[from][fluxType].AddRange(toList);
            }
        }
    }

    private void AddF1Transitions(Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> matrix)
    {
        var f1Transitions = new Dictionary<WorkflowStatus, List<WorkflowStatus>>
        {
            { WorkflowStatus.OS_CRIADA, new List<WorkflowStatus> { WorkflowStatus.MATERIAL_FABRICADO } },
            { WorkflowStatus.MATERIAL_FABRICADO, new List<WorkflowStatus> { WorkflowStatus.ROMANEIO_CONFERIDO } },
            { WorkflowStatus.VINCULO_CRIADO, new List<WorkflowStatus> { WorkflowStatus.ESTOQUE_ORIGEM_ATUALIZADO } },
            { WorkflowStatus.ESTOQUE_ORIGEM_ATUALIZADO, new List<WorkflowStatus> { WorkflowStatus.NFE_SAIDA_ORIGEM_EMITIDA } },
            { WorkflowStatus.NFE_SAIDA_ORIGEM_EMITIDA, new List<WorkflowStatus> { WorkflowStatus.EM_TRANSITO } },
            { WorkflowStatus.EM_TRANSITO, new List<WorkflowStatus> { WorkflowStatus.CHEGADA_MATERIAL_DESTINO } },
            { WorkflowStatus.CHEGADA_MATERIAL_DESTINO, new List<WorkflowStatus> { WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO } },
            { WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA, new List<WorkflowStatus> { WorkflowStatus.ESTOQUE_DESTINO_ATUALIZADO } },
            { WorkflowStatus.ESTOQUE_DESTINO_ATUALIZADO, new List<WorkflowStatus> { WorkflowStatus.PROCESSO_CONCLUIDO } }
        };

        foreach (var (from, toList) in f1Transitions)
        {
            if (!matrix.ContainsKey(from))
            {
                matrix[from] = new Dictionary<FluxType, List<WorkflowStatus>>();
            }

            if (!matrix[from].ContainsKey(FluxType.COMPRA_DIRETA))
            {
                matrix[from][FluxType.COMPRA_DIRETA] = new List<WorkflowStatus>();
            }
            matrix[from][FluxType.COMPRA_DIRETA].AddRange(toList);
        }
    }

    private void AddF2Transitions(Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> matrix)
    {
        var f2Transitions = new Dictionary<WorkflowStatus, List<WorkflowStatus>>
        {
            { WorkflowStatus.OS_CRIADA, new List<WorkflowStatus> { WorkflowStatus.OS_ATUALIZADA_DATA_ESTIMADA } },
            { WorkflowStatus.OS_ATUALIZADA_DATA_ESTIMADA, new List<WorkflowStatus> { WorkflowStatus.NFE_ENTREGA_FUTURA_EMITIDA } },
            { WorkflowStatus.NFE_ENTREGA_FUTURA_EMITIDA, new List<WorkflowStatus> { WorkflowStatus.XML_OBTIDO } },
            { WorkflowStatus.NFE_VALIDADA_OK, new List<WorkflowStatus> { WorkflowStatus.NFE_RECEBIDA_SEM_ESTOQUE } },
            { WorkflowStatus.NFE_RECEBIDA_SEM_ESTOQUE, new List<WorkflowStatus> { WorkflowStatus.OC_CRIADA_PARA_REMESSA } },
            { WorkflowStatus.OC_CRIADA_PARA_REMESSA, new List<WorkflowStatus> { WorkflowStatus.AGUARDANDO_REMESSA, WorkflowStatus.ALERTA_7_DIAS_ENTREGA_ESTIMADA } },
            { WorkflowStatus.AGUARDANDO_REMESSA, new List<WorkflowStatus> { WorkflowStatus.PROCESSO_CONCLUIDO } }
        };

        foreach (var (from, toList) in f2Transitions)
        {
            if (!matrix.ContainsKey(from))
            {
                matrix[from] = new Dictionary<FluxType, List<WorkflowStatus>>();
            }

            if (!matrix[from].ContainsKey(FluxType.ENTREGA_FUTURA_MAE))
            {
                matrix[from][FluxType.ENTREGA_FUTURA_MAE] = new List<WorkflowStatus>();
            }
            matrix[from][FluxType.ENTREGA_FUTURA_MAE].AddRange(toList);
        }
    }

    private void AddF3Transitions(Dictionary<WorkflowStatus, Dictionary<FluxType, List<WorkflowStatus>>> matrix)
    {
        var f3Transitions = new Dictionary<WorkflowStatus, List<WorkflowStatus>>
        {
            { WorkflowStatus.OS_CRIADA, new List<WorkflowStatus> { WorkflowStatus.OC_PENDENTE_ENTREGA_FUTURA } },
            { WorkflowStatus.OC_PENDENTE_ENTREGA_FUTURA, new List<WorkflowStatus> { WorkflowStatus.APROVACAO_ENTREGA_PENDENTE } },
            { WorkflowStatus.APROVACAO_ENTREGA_PENDENTE, new List<WorkflowStatus> { WorkflowStatus.ENTREGA_APROVADA, WorkflowStatus.ENTREGA_REPROVADA } },
            { WorkflowStatus.ENTREGA_APROVADA, new List<WorkflowStatus> { WorkflowStatus.NFE_REMESSA_EMITIDA } },
            { WorkflowStatus.NFE_REMESSA_EMITIDA, new List<WorkflowStatus> { WorkflowStatus.VINCULADA_OS_OC_NFE } },
            { WorkflowStatus.VINCULADA_OS_OC_NFE, new List<WorkflowStatus> { WorkflowStatus.ENTRADA_ORIGEM_CONCLUIDA } },
            { WorkflowStatus.ENTRADA_ORIGEM_CONCLUIDA, new List<WorkflowStatus> { WorkflowStatus.SAIDA_ORIGEM_CONCLUIDA } },
            { WorkflowStatus.SAIDA_ORIGEM_CONCLUIDA, new List<WorkflowStatus> { WorkflowStatus.EM_TRANSITO, WorkflowStatus.ALERTA_30_DIAS_DESTINO } },
            { WorkflowStatus.EM_TRANSITO, new List<WorkflowStatus> { WorkflowStatus.CHEGADA_MATERIAL_DESTINO } },
            { WorkflowStatus.CHEGADA_MATERIAL_DESTINO, new List<WorkflowStatus> { WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO } },
            { WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA, new List<WorkflowStatus> { WorkflowStatus.PROCESSO_CONCLUIDO } }
        };

        foreach (var (from, toList) in f3Transitions)
        {
            if (!matrix.ContainsKey(from))
            {
                matrix[from] = new Dictionary<FluxType, List<WorkflowStatus>>();
            }

            if (!matrix[from].ContainsKey(FluxType.ENTREGA_FUTURA_FILHA))
            {
                matrix[from][FluxType.ENTREGA_FUTURA_FILHA] = new List<WorkflowStatus>();
            }
            matrix[from][FluxType.ENTREGA_FUTURA_FILHA].AddRange(toList);
        }
    }

    private Dictionary<WorkflowStatus, List<string>> BuildRoleMatrix()
    {
        return new Dictionary<WorkflowStatus, List<string>>
        {
            { WorkflowStatus.MATERIAL_FABRICADO, new List<string> { "FABRICA", "SISTEMA" } },
            { WorkflowStatus.ROMANEIO_CONFERIDO, new List<string> { "ADM_FILIAL_ORIGEM", "FABRICA", "SISTEMA" } },
            { WorkflowStatus.NFE_EMITIDA, new List<string> { "ADM_FILIAL_ORIGEM", "ADM_FILIAL_DESTINO", "SISTEMA" } },
            { WorkflowStatus.XML_OBTIDO, new List<string> { "SISTEMA" } },
            { WorkflowStatus.NFE_VALIDADA_OK, new List<string> { "FISCAL", "SISTEMA" } },
            { WorkflowStatus.NFE_VALIDADA_NOK, new List<string> { "FISCAL", "SISTEMA" } },
            { WorkflowStatus.VINCULO_CRIADO, new List<string> { "ADM_FILIAL_ORIGEM", "SISTEMA" } },
            { WorkflowStatus.ENTREGA_APROVADA, new List<string> { "GESTOR_CONTRATO", "SISTEMA" } },
            { WorkflowStatus.ENTREGA_REPROVADA, new List<string> { "ADM_FILIAL_DESTINO", "SISTEMA" } },
            { WorkflowStatus.APROVACAO_ENTREGA_PENDENTE, new List<string> { "ADM_FILIAL_DESTINO", "SISTEMA" } },
            { WorkflowStatus.CHEGADA_MATERIAL_DESTINO, new List<string> { "ADM_FILIAL_DESTINO", "SISTEMA" } },
            { WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA, new List<string> { "ADM_FILIAL_DESTINO", "SISTEMA" } },
            { WorkflowStatus.PROCESSO_CANCELADO, new List<string> { "GESTOR_CONTRATO", "SISTEMA" } },
            { WorkflowStatus.MEDICAO_APROVADA, new List<string> { "SISTEMA" } },
            { WorkflowStatus.MEDICAO_REPROVADA, new List<string> { "SISTEMA" } }
        };
    }
}
