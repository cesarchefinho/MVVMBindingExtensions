using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace CommunityToolkit.Mvvm.BindingExtensions;

public partial class MVVMBaseViewModel : ObservableValidator
{
    public MVVMBaseViewModel()
    {
        this.ErrorsChanged += OnErrorsChanged;
    }
    public void ValidateAll()
    {
        base.ValidateAllProperties();
    }

    [JsonInclude]
    public string Errors => string.Join(Environment.NewLine, 
                                        from ValidationResult e 
                                          in GetErrors() 
                                      select e.ErrorMessage);
   
    [JsonIgnore]
    [IndexerName("Item")]
    public string this[string columnName] =>
        string.Join(Environment.NewLine,
                    (from ValidationResult e 
                       in GetErrors(columnName) 
                   select e.ErrorMessage));

    public void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
        {
            OnPropertyChanged("Item[]");
        }
        else
        {
            OnPropertyChanged($"Item[{e.PropertyName}]");
        }
    }
}
