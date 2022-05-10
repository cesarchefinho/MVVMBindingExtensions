using System;
using ComponetModel.DataAnnotaions;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

    public partial class LoginViewModel : MVVMBaseViewModel
    {       
   	[Display(Name = "User Id", Description = "Your UserId in format of 999.999.999-99")]
        [DisplayFormat(DataFormatString = "999.999.999-99")]
        [NotEmpty(ErrorMessage = "UserId is required", EmptyValue = "   .   .   -  ")]
        [StringLength(maximumLength: 14, MinimumLength = 11, ErrorMessage = "Need between 11 or 14 chars!")]
        // [UserId] => Some validatorAttribute
        [ObservableProperty]        
        private string userId = String.Empty;

       [Display(Name = "Password", Description = "Type here your Passowrd")]
       [DataType(DataType.Password)]
       [Required(ErrorMessage = "The Password is required!")]
       [StringLength(maximumLength: 16, MinimumLength = 6, ErrorMessage = "minimum of 6 and maximum of 16 chars !")]
       [Password]
       [ObservableProperty]
       private string password = String.Empty;
 
       [Display(Name = "Newsletter", Description = "Marque esta opção indicando que você aceita receber nossos emails.")]
       [IsCheckedAttribute ("It is required accept receive emails to Login.")]
       [ObservableProperty]
       private bool newsletter;

       [ICommand]
       private async Task Login()
       {
          ValidateAll ();

          if (! HasErrors)
          {
             // Do Something
          }
    }

