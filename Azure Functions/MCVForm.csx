// MCVForm.csx
// Yong-Jun Shin
// 2018

using System;
using Microsoft.Bot.Builder.FormFlow;

// For more information about this template visit http://aka.ms/azurebots-csharp-form
[Serializable]
public class MCVForm
{
    [Prompt("What is your MCV in femtoliter (fL)? (e.g., 86)")]
    public double MCV { get; set; }

    public static IForm<MCVForm> BuildForm()
    {
        // Builds an IForm<T> based on BasicForm
        return new FormBuilder<MCVForm>().Build();
    }

    public static IFormDialog<MCVForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
    {
        // Generated a new FormDialog<T> based on IForm<BasicForm>
        return FormDialog.FromForm(BuildForm, options);
    }
}
