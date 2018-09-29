//HemoglobinForm.csx
using System;
using Microsoft.Bot.Builder.FormFlow;

public enum GenderOptions { Female = 1, Male};

// For more information about this template visit http://aka.ms/azurebots-csharp-form
[Serializable]
public class HemoglobinForm
{
    [Prompt("What is your patient ID (e.g., pt273)?")]
    public string PatientID { get; set; }
    
    [Prompt("Normal levels of hemoglobin depend on sex and age. Please select one. {||}")]
    public GenderOptions Gender { get; set; }

    [Prompt("What is your age?")]
    public int Age { get; set; }
    
    [Prompt("What is your hemoglobin level (g/dL)? (e.g., 9.8)")]
    public double HemoglobinValue { get; set; }

    public static IForm<HemoglobinForm> BuildForm()
    {
        // Builds an IForm<T> based on BasicForm
        return new FormBuilder<HemoglobinForm>().Build();
    }

    public static IFormDialog<HemoglobinForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
    {
        // Generated a new FormDialog<T> based on IForm<BasicForm>
        return FormDialog.FromForm(BuildForm, options);
    }
}
