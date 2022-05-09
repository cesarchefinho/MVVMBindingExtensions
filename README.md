# MVVMBindingExtensions

XAML Binding Extension to Bind View Models Properties anotadded with ComponentModel.DataAnnotations Attributes 

Works on UNO, WinUI3 and UWP

An Extension to XAML language that simplify markup and expand XAML capabilities
by automatcaly bind control to ViewModel, display validation error messages individualy per control,
and read all possible ViewModel Annotations to control without any coding

things like this

               < ! -- (PasswordBox , but any control, like Textbox, combobox, radio buttons, etc) -- >

               < PasswordBox 
                           Header="read DisplayName from anotations on view model"
                           Description=""read Error on viewmodel validation events"
                           PlaceholderText="read DisplayDescription from anotations on view model"
                           TextboxMask="read DisplayFormat from anotations on view model"
                           InputScope="Read best input scope based on anotations and viewmodel property"
                           Password="{x:Bind ViewModel.PasswordProp}"
                           MaxLength=""read all validation attributes from anotations on view model"
                          etc...etc...etc
                        / > 
                        
simple will be this in markup: 

       < PasswordBox be:MVVM.ViewModel="{x:Bind ViewModel}"  
                     be:MVVM.PropertyName="PasswordProp" / >
                               
                               
The be:MVM extensions make all binding to you based on anottations of view model
also the be.MVVM extension use ObservableValidator and ObservablePropertyes
also the be:MVVM resolve binding to indexed string array attribute of collumns errors (bug in winui3 workedarrounded by be:MVVM)
also suport display error messages in TextBlock other than description property for controls that dont have Description

example:

     
 < CheckBox   

        be:MVVM.ErrorControl="{x:Bind NewsletterErrorControl}" 
        be:MVVM.ViewModel="{x:Bind ViewModel.DadosCadastro}" 
        be:MVVM.PropertyName="Newsletter"
        Content="I Accept receive Emails."/ >


     < TextBlock Foreground="DarkRed" 
                 x:Name="AcceptErrorControl" / >
                      
ViewModel:

public partial class UserViewModel : MVVMBaseViewModel 
{

        [Display(Name = "Password", Description = "Type your password Here")]
        [Password]
        [Required(ErrorMessage = "Please, Type your Password.")]
        [StringLength(maximumLength: 16, MinimumLength = 6, ErrorMessage = "Password must be at minumum 6 chars e maximum of 16 chars !")]
        [ObservableProperty]
        private string passwordProp = String.Empty;
        
        
        [Display(Name = "Newsletter", Description = "Marque esta opção indicando que você aceita receber nossos emails.")]
        [IsCheckedAttribute ("It is mandatory accept receive emails to continue.")]
        [ObservableProperty]
        private bool newsletter;
}


MVVM Bind Extensions automaticaly bind these properties based on ViewModel Annotations:

InputScope   => Binded with InpuScope Based on DataTypeAttribute or from Validators or by DataFormat (9, A, etc Masks)

Header       => Binded with value readed from DIsplayNameAttribute.Name

TExt / Password/ SelectedValue => Binded to property indicated by Propertyname  

Description => Binded to validation error of Windows Community Toolkit ValidationObject error property

Tooltip => Binded with value Readed from DIsplayNameAttribute.Description

PlaceHolder => Binded with value Readed from DataFormat Format otherwise from DIsplayNameAttribute.Description

MaxLenght   => Binded with value Readed from StringLengthAttribute or from MaxLengthAttribute

IsReadOnly  => Binded with value Readed from EditableAttribute

MinValue / Minimum => Binded with value Readed from RangeAttribute

MaxValue / Maximum => Binded with value Readed from RangeAttribute 

CharacterCasing =>  Binded with value Readed from UpperCaseAttribute / LowerCaseAttribute

Mask => Binded with value Readed from DataFormatttribute DisplayFormat 


User Customization:


You can tell MVVM Binding extensions at app Startup how to Bind Controls and decect proper InputScopes using 


   MVVM.InputScopesOfValidators.Add { typeof (Validator), InputScopeNameValue});
   
   
   and
   
   
   MVVM.DataDependencyProperties.Add ( typeof (Control), DependencyPropertyOfControl});


   ie: MVVM.DataDependencyProperties.Add ( typeof (CustomTextBox), CustomTextBox.TextDependencyProperty});
   
   
Alternativaly you can bind DataAnotations individualy with MarkupExtensions like

.... xmlns:be="using:CommunityToolkit.Mvvm.BindingExtensions"


   < TextBox   Text={x:Bind ViewModel.Username, Mode=TwoWay}
               Header={be:DisplayNameOf ViewModel=ViewModel, PropertyName="UserName"}
               TextBox.Tooltip={be:DescriptionOf ViewModel=ViewModel, PropertyName="UserName"}
               Description={be:ErrorOf ViewModel=ViewModel, PropertyName="UserName"}
               PlaceHolder={be:DisplayFormatOf ViewModel=ViewModel, PropertyName="UserName"}
               MinLength={be:MinLengthOf ViewModel=ViewModel, PropertyName="UserName"}
               MaxLength={be:MaxLengthOf ViewModel=ViewModel, PropertyName="UserName"}
       / >	
   
TODO


    Make a version without reflection, to improve performance, may be using source generators
    
    
