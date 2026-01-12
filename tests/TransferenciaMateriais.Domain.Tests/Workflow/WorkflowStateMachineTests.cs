using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Domain.Workflow;
using Xunit;

namespace TransferenciaMateriais.Domain.Tests.Workflow;

public class WorkflowStateMachineTests
{
    private readonly IWorkflowStateMachine _stateMachine;

    public WorkflowStateMachineTests()
    {
        _stateMachine = new WorkflowStateMachine();
    }

    [Fact]
    public void CanTransition_FromOsCriada_ToRomaneioConferido_ShouldSucceed()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.OS_CRIADA,
            ToStatus = WorkflowStatus.ROMANEIO_CONFERIDO,
            ActorRole = "ADM_FILIAL_ORIGEM"
        };

        // Act
        var result = _stateMachine.CanTransition(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CanTransition_FromOsCriada_ToMaterialFabricado_F1_ShouldSucceed()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.OS_CRIADA,
            ToStatus = WorkflowStatus.MATERIAL_FABRICADO,
            ActorRole = "FABRICA"
        };

        // Act
        var result = _stateMachine.CanTransition(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CanTransition_InvalidRole_ShouldFail()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.OS_CRIADA,
            ToStatus = WorkflowStatus.MATERIAL_FABRICADO,
            ActorRole = "FISCAL" // Papel incorreto
        };

        // Act
        var result = _stateMachine.CanTransition(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("não tem permissão", result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public void ValidateBusinessRules_EntradaDestinoConcluida_WithoutAttachment_ShouldFail()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO,
            ToStatus = WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA,
            ActorRole = "ADM_FILIAL_DESTINO",
            Context = new Dictionary<string, object> { { "hasRequiredAttachment", false } }
        };

        // Act
        var result = _stateMachine.ValidateBusinessRules(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("RB-04", result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public void ValidateBusinessRules_EntradaDestinoConcluida_WithAttachment_ShouldSucceed()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO,
            ToStatus = WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA,
            ActorRole = "ADM_FILIAL_DESTINO",
            Context = new Dictionary<string, object> { { "hasRequiredAttachment", true } }
        };

        // Act
        var result = _stateMachine.ValidateBusinessRules(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateBusinessRules_NfeValidadaNok_WithoutReason_ShouldFail()
    {
        // Arrange
        var request = new WorkflowTransitionRequest
        {
            OsId = Guid.NewGuid(),
            FromStatus = WorkflowStatus.XML_OBTIDO,
            ToStatus = WorkflowStatus.NFE_VALIDADA_NOK,
            ActorRole = "FISCAL",
            Reason = null // RB-05: motivo obrigatório
        };

        // Act
        var result = _stateMachine.ValidateBusinessRules(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("RB-05", result.ErrorMessage ?? string.Empty);
    }

    [Fact]
    public void GetAllowedTransitions_FromOsCriada_F1_ShouldReturnValidTransitions()
    {
        // Act
        var transitions = _stateMachine.GetAllowedTransitions(
            WorkflowStatus.OS_CRIADA,
            FluxType.COMPRA_DIRETA,
            "FABRICA"
        );

        // Assert
        Assert.Contains(WorkflowStatus.MATERIAL_FABRICADO, transitions);
        Assert.Contains(WorkflowStatus.ROMANEIO_CONFERIDO, transitions);
    }

    [Fact]
    public void GetAllowedTransitions_FromOsCriada_F2_ShouldReturnValidTransitions()
    {
        // Act
        var transitions = _stateMachine.GetAllowedTransitions(
            WorkflowStatus.OS_CRIADA,
            FluxType.ENTREGA_FUTURA_MAE,
            "ADM_FILIAL_ORIGEM"
        );

        // Assert
        Assert.Contains(WorkflowStatus.OS_ATUALIZADA_DATA_ESTIMADA, transitions);
    }

    [Fact]
    public void GetAllowedTransitions_FromOsCriada_F3_ShouldReturnValidTransitions()
    {
        // Act
        var transitions = _stateMachine.GetAllowedTransitions(
            WorkflowStatus.OS_CRIADA,
            FluxType.ENTREGA_FUTURA_FILHA,
            "ADM_FILIAL_ORIGEM"
        );

        // Assert
        Assert.Contains(WorkflowStatus.OC_PENDENTE_ENTREGA_FUTURA, transitions);
    }
}
