using System.Text;
using DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;
using DotCruz.Garage.Domain.Exceptions.Resources;

namespace UseCases.Test.Catalog;

public class ExtractManualMaintenanceCommandValidatorTest
{
    [Fact]
    public void Valid_Command_Passes_Validation()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: stream,
            FileName: "manual.pdf",
            Brand: "Toyota",
            Model: "Corolla",
            Year: 2024,
            Engine: "2.0",
            Version: "Altis");

        var validator = new ExtractManualMaintenanceCommandValidator();
        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_Brand_Or_Model_Fails_Validation()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: stream,
            FileName: "manual.pdf",
            Brand: "",
            Model: "",
            Year: 2024,
            Engine: null,
            Version: null);

        var validator = new ExtractManualMaintenanceCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.BRAND_EMPTY);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.MODEL_EMPTY);
    }

    [Fact]
    public void Non_Pdf_File_Fails_Validation()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var command = new ExtractManualMaintenanceCommand(
            FileStream: stream,
            FileName: "manual.docx",
            Brand: "Toyota",
            Model: "Corolla",
            Year: 2024,
            Engine: null,
            Version: null);

        var validator = new ExtractManualMaintenanceCommandValidator();
        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ResourceMessagesException.MANUAL_FILE_INVALID_FORMAT);
    }
}
