using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.Mvvm.BindingExtensions;
public static class MVVM
{
    public static Dictionary<Type, InputScopeNameValue> InputScopesOfValidators { get; set; } = new Dictionary<Type, InputScopeNameValue>()
    {
        { typeof (EmailAddressAttribute),  InputScopeNameValue.EmailNameOrAddress},
        { typeof (UrlAttribute),           InputScopeNameValue.Url},
        { typeof (CreditCardAttribute),    InputScopeNameValue.NumberFullWidth},
        { typeof (PhoneAttribute),         InputScopeNameValue.NumberFullWidth},
        { typeof (FullNameAttribute),      InputScopeNameValue.PersonalFullName},
        { typeof (PasswordAttribute),      InputScopeNameValue.Password},
    };

    public static Dictionary<DataType, InputScopeNameValue> InputScopesOfDataTypes { get; set; } = new Dictionary<DataType, InputScopeNameValue>()
    {
        { DataType.CreditCard,    InputScopeNameValue.NumberFullWidth},
        { DataType.Currency,      InputScopeNameValue.NumberFullWidth},
        { DataType.Date,          InputScopeNameValue.NumberFullWidth},
        { DataType.DateTime,      InputScopeNameValue.NumberFullWidth},
        { DataType.Duration,      InputScopeNameValue.NumberFullWidth},
        { DataType.EmailAddress,  InputScopeNameValue.EmailNameOrAddress},
        { DataType.Html,          InputScopeNameValue.Text},
        { DataType.ImageUrl,      InputScopeNameValue.Url},
        { DataType.MultilineText, InputScopeNameValue.Text},
        { DataType.Password,      InputScopeNameValue.Password},
        { DataType.PhoneNumber,   InputScopeNameValue.NumberFullWidth},
        { DataType.PostalCode,    InputScopeNameValue.NumberFullWidth},
        { DataType.Text,          InputScopeNameValue.Text},
        { DataType.Time,          InputScopeNameValue.NumberFullWidth},
        { DataType.Upload,        InputScopeNameValue.Url},
        { DataType.Url,           InputScopeNameValue.Url},
    };

    private static InputScope GetInputScope (   PropertyInfo prop,
                                                DataTypeAttribute attr_data_type,
                                                string DataFormat)
    {
        try
        {
            InputScope scope = new InputScope();

            // Checks inputscope by datatype attribute

            if (attr_data_type != null)
            {
                if (InputScopesOfDataTypes.ContainsKey(attr_data_type.DataType))
                {
                    scope.Names.Add(new InputScopeName
                    {
                        NameValue = InputScopesOfDataTypes [attr_data_type.DataType]
                    });

                    return scope;
                }
            }

            // Checks inputscope by common validadtions attribute

            foreach (Type tp in InputScopesOfValidators.Keys)
            {
                if (prop.GetCustomAttributes(tp, true).Any())
                {
                    scope.Names.Add(new InputScopeName
                    {
                        NameValue = InputScopesOfValidators[tp]
                    });

                    return scope;
                }
            }

            // Checks inputscope by common dataform attribute

            if (!string.IsNullOrWhiteSpace(DataFormat) && scope.Names.Count < 1)
            {
                bool FlagInputScopeDigit = false;
                bool FlagInputScopeAlpha = false;

                foreach (char ch in DataFormat.Distinct())
                {
                    if (// ch == '9' ||
                        char.IsDigit(ch) ||
                        ch == '+' ||
                        ch == '-' ||
                        ch == '/' ||
                        ch == '*' ||
                        ch == '=')
                    {
                        FlagInputScopeDigit = true;
                        // continue scan until the end of string
                    }
                    else
                    {
                        // ch == 'A' || 
                        // ch == 'a' ||
                        // ch == '_' ||
                        // ch == '?' ||
                        // char.IsLetter(ch) ||
                        // char.IsWhiteSpace(ch)
                        // ANy other caracter

                        FlagInputScopeAlpha = true;
                        // it is alpha, stop string scan
                        break;
                    }
                }

                if (FlagInputScopeAlpha) // Even if FlagInputScopeDigit is trua
                {
                    scope.Names.Add(new InputScopeName
                    {
                        NameValue = InputScopeNameValue.Default
                    });

                    return scope;
                }
                else if (FlagInputScopeDigit) // Onlu when not alpha
                {
                    scope.Names.Add(new InputScopeName
                    {
                        NameValue = InputScopeNameValue.CurrencyAmount
                    });

                    return scope;
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.GetInputScope() : {ex.Message} \r\n");
        }

        return null;
    }

    public static Dictionary<Type, DependencyProperty> DataDependencyProperties { get; set; } = new Dictionary<Type, DependencyProperty>()
    {
        { typeof (PasswordBox),         PasswordBox.PasswordProperty},
        { typeof (AutoSuggestBox),      AutoSuggestBox.TextProperty},
        { typeof (NumberBox),           NumberBox.TextProperty},
        { typeof (RatingControl),       RatingControl.ValueProperty},
        { typeof (RadioButtons),        RadioButtons.SelectedItemProperty},
        { typeof (Slider),              Slider.ValueProperty},
        { typeof (ComboBox),            ComboBox.SelectedValueProperty},
        { typeof (ListBox),             ListBox.SelectedValueProperty},
        { typeof (CheckBox),            CheckBox.IsCheckedProperty},
        { typeof (RadioButton),         RadioButton.IsCheckedProperty},
        { typeof (DatePicker),          DatePicker.DateProperty},
        { typeof (CalendarDatePicker),  CalendarDatePicker.DateProperty},
        { typeof (TextBox),             TextBox.TextProperty},
#if __WINDOWS__ || __WINDOWS_UWP__
        { typeof (TimePicker),          TimePicker.TimeProperty},
#endif
    };
    private static void BindErrorMessage(DependencyObject d, 
                                         TextBlock errorctrl, 
                                         string PropertyName,
                                         object ViewModel)
    {
        try 
        {
            PropertyInfo Errorprop = ViewModel.GetType().GetProperty("Item");

            if (Errorprop != null)
            {
                string ErrorPropertyName = $"[{PropertyName}]";

                Binding myBindingError = new Binding()
                {
                    Source = ViewModel,
                    Path = new PropertyPath(ErrorPropertyName),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };

                if (errorctrl != null)
                {
                    BindingOperations.SetBinding(errorctrl, TextBlock.TextProperty, myBindingError);
                }
                else
                {
                    PropertyInfo piErrorDependencyProperty = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "DescriptionProperty");

                    if (piErrorDependencyProperty != null)
                    {
                        DependencyProperty ErrorDependencyProperty = (DependencyProperty)piErrorDependencyProperty.GetValue(d);

                        if (ErrorDependencyProperty != null)
                        {
                            BindingOperations.SetBinding(d, ErrorDependencyProperty, myBindingError);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.BindErrorMessage() : {ex.Message} \r\n");
        }
    }

    private static void BindDataAnotations(DependencyObject d)
    {
        try 
        {
            if (d != null)
            {
                string PropertyName = MVVM.GetPropertyName(d);

                object ViewModel = MVVM.GetViewModel(d);

                if (ViewModel != null && !string.IsNullOrWhiteSpace(PropertyName))
                {
                    Type type = ViewModel.GetType();

                    PropertyInfo prop = type.GetProperty(PropertyName);

                    if (prop != null)
                    {
                        string fieldname = char.ToLower(PropertyName[0]).ToString();

                        if (PropertyName.Length > 1)
                        {
                            fieldname += PropertyName.Substring(1);
                        }

                        FieldInfo fi = type.GetField(fieldname, BindingFlags.NonPublic | BindingFlags.Instance);

                        // 
                        // Adjust Header / Caption
                        //

                        DisplayAttribute attr_desc = prop?.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;

                        if (attr_desc == null)
                        {
                            attr_desc = fi?.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                        }

                        string DisplayName = attr_desc != null ? attr_desc.Name : null;

                        PropertyInfo piHeader = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "Header");

                        if (piHeader != null)
                        {
                            if (DisplayName != null)
                            {
                                piHeader.SetValue(d, DisplayName);
                            }
                        }
                        else
                        {
                            PropertyInfo piCaption = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "Caption");

                            if (piCaption != null && DisplayName != null)
                            {
                                piCaption.SetValue(d, DisplayName);
                            }
                        }

                        //
                        // Bind Data Property 
                        //

                        if (DataDependencyProperties.ContainsKey(d.GetType()))
                        {
                            DependencyProperty DataProperty = DataDependencyProperties [d.GetType()];

                            Binding myBinding = new Binding()
                            {
                                Source = ViewModel,
                                Path = new PropertyPath(PropertyName),
                                Mode = BindingMode.TwoWay,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            };

                            BindingOperations.SetBinding(d, DataProperty, myBinding);
                        }

                        //
                        // Bind Error Messages
                        //

                        TextBlock errorctrl = MVVM.GetErrorControl(d);

                        BindErrorMessage(d, errorctrl, PropertyName, ViewModel);

                        //
                        // Bind Tooltip
                        //

                        string Description = attr_desc != null ? attr_desc.Description : null;

                        if (Description != null)
                        {
                            ToolTip toolTip = new ToolTip
                            {
                                Content = Description
                            };

                            ToolTipService.SetToolTip(d, toolTip);
                        }

                        // 
                        // Bind PlaceHolder
                        //

                        DisplayFormatAttribute attr_format = prop?.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault() as DisplayFormatAttribute;

                        if (attr_format == null)
                        {
                            attr_format = fi?.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault() as DisplayFormatAttribute;
                        }

                        string PlaceHolder = attr_format != null ? attr_format.DataFormatString : attr_desc != null ? attr_desc.Name : null;

                        PropertyInfo piPlaceHolder = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "PlaceholderText");

                        if (piPlaceHolder != null && PlaceHolder != null)
                        {
                            piPlaceHolder.SetValue(d, PlaceHolder);
                        }

                        //
                        // Bind MaxLenght
                        //

                        StringLengthAttribute attr_str_length = prop.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;
                        MaxLengthAttribute attr_max_length = prop.GetCustomAttributes(typeof(MaxLengthAttribute), true).FirstOrDefault() as MaxLengthAttribute;

                        PropertyInfo piMaxLength = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "MaxLength");

                        if (piMaxLength != null)
                        {
                            if (attr_str_length != null)
                            {
                                piMaxLength.SetValue(d, attr_str_length.MaximumLength);
                            }

                            if (attr_max_length != null)
                            {
                                piMaxLength.SetValue(d, attr_max_length.Length);
                            }
                        }

                        //
                        // Bind IsReadOnly
                        //

                        EditableAttribute attr_editable = prop?.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;

                        if (attr_editable == null)
                        {
                            attr_editable = fi?.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
                        }

                        PropertyInfo piIsReadOnly = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "IsReadOnly");

                        if (piIsReadOnly != null)
                        {
                            if (attr_editable != null)
                            {
                                piIsReadOnly.SetValue(d, attr_editable.AllowEdit);
                            }
                        }
                        else
                        {
                            PropertyInfo piIsEnabled = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "IsEnabled");

                            if (piIsEnabled != null && attr_editable != null)
                            {
                                piIsEnabled.SetValue(d, attr_editable.AllowEdit);
                            }
                        }

                        //
                        // Bind MinValue / Minimum
                        //

                        RangeAttribute attr_range = prop.GetCustomAttributes(typeof(RangeAttribute), true).FirstOrDefault() as RangeAttribute;

                        double? MinValue = attr_range != null ? attr_range.Minimum as double? : null;

                        PropertyInfo piMinValue = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "MinValue" || x.Name == "Minimum");

                        if (piMinValue != null && MinValue != null)
                        {
                            piMinValue.SetValue(d, MinValue);
                        }

                        //
                        // Bind MaxValue / Maximum
                        //

                        double? MaxValue = attr_range != null ? attr_range.Maximum as double? : null;

                        PropertyInfo piMaxValue = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "MaxValue" || x.Name == "Maximum");

                        if (piMaxValue != null && MaxValue != null)
                        {
                            piMaxValue.SetValue(d, MaxValue);
                        }


                        //
                        // Bind CharacterCasing "
                        // 

#if WINDOWS || WINDOWS_UWP
                        PropertyInfo piCharacterCasing = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "CharacterCasing");

                        UpperCaseAttribute upper_case = prop.GetCustomAttributes(typeof(UpperCaseAttribute), true).FirstOrDefault() as UpperCaseAttribute;

                        if (upper_case == null)
                        {
                            upper_case = fi?.GetCustomAttributes(typeof(UpperCaseAttribute), true).FirstOrDefault() as UpperCaseAttribute;
                        }

                        LowerCaseAttribute lower_case = prop.GetCustomAttributes(typeof(LowerCaseAttribute), true).FirstOrDefault() as LowerCaseAttribute;

                        if (lower_case == null)
                        {
                            lower_case = fi?.GetCustomAttributes(typeof(LowerCaseAttribute), true).FirstOrDefault() as LowerCaseAttribute;
                        }

                        if (piCharacterCasing != null)
                        {
                            //
                            // Adjust CharacterCasing="Upper"
                            // 

                            if (upper_case != null)
                            {
                                piCharacterCasing.SetValue(d, CharacterCasing.Upper);
                            }

                            //
                            // Adjust CharacterCasing="Lower"
                            // 

                            if (lower_case != null)
                            {
                                piCharacterCasing.SetValue(d, CharacterCasing.Lower);
                            }
                        }
#endif
                        // 
                        // Bind InputScope
                        //

                        DataTypeAttribute attr_data_type = prop?.GetCustomAttributes(typeof(DataTypeAttribute), true).FirstOrDefault() as DataTypeAttribute;

                        if (attr_data_type == null)
                        {
                            attr_data_type = fi?.GetCustomAttributes(typeof(DataTypeAttribute), true).FirstOrDefault() as DataTypeAttribute;
                        }

                        string DataFormat = attr_format != null ? attr_format.DataFormatString : string.Empty;

                        InputScope scope = GetInputScope(prop, attr_data_type, DataFormat);

                        PropertyInfo piInputScope = d.GetType().GetProperties().FirstOrDefault(x => x.Name == "InputScope" || x.Name == "TextBoxInputScope");

                        if (piInputScope != null && scope != null)
                        {
                            piInputScope.SetValue(d, scope);
                        }

                        //
                        // Mask textbox from DisplayFormat
                        //

                        if (d is TextBox)
                        {
                            //
                            // I dont use mask extensions from CommunityToolkit Beacause it dont work with uno
                            // Aleternative I maded an MaskExtension here that works on Uno too
                            //

                            if (!string.IsNullOrWhiteSpace(DataFormat))
                            {
                                MVVM.SetInputMask(d as TextBox, DataFormat);
                            }
                        }

                        //
                        // if AutoSuggestBox, Adjust Textbox inside AutoSuggestBox
                        //

                        if (d is AutoSuggestBox)
                        {
                            AutoSuggestBox elem = d as AutoSuggestBox;

                            TextBox elem_textbox = MVVM.FindChildAutoSuggestBox<TextBox>(elem);

                            if (elem_textbox != null)
                            {
                                if (scope != null)
                                {
                                    elem_textbox.InputScope = scope;
                                }

                                if (attr_str_length != null)
                                {
                                    elem_textbox.MaxLength = attr_str_length.MaximumLength;
                                }

                                if (attr_max_length != null)
                                {
                                    elem_textbox.MaxLength = attr_max_length.Length;
                                }

                                if (attr_editable != null)
                                {
                                    elem_textbox.IsReadOnly = !attr_editable.AllowEdit;
                                }

#if WINDOWS || WINDOWS_UWP
                                //
                                // Adjust CharacterCasing="Upper"
                                // 

                                if (upper_case != null)
                                {
                                    elem_textbox.CharacterCasing = CharacterCasing.Upper;
                                }

                                //
                                // Adjust CharacterCasing="Lower"
                                // 

                                if (lower_case != null)
                                {
                                    elem_textbox.CharacterCasing = CharacterCasing.Lower;
                                }
#endif

                                //
                                // Mask textbox from DisplayFormat
                                //

                                if (!string.IsNullOrWhiteSpace(DataFormat))
                                {
                                    MVVM.SetInputMask(elem_textbox, DataFormat);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.BindDataAnotations() : {ex.Message} \r\n");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.RegisterAttached("ViewModel", typeof(object), typeof(MVVM), new PropertyMetadata(null, OnViewModelChanged));

    public static object GetViewModel(DependencyObject d)
    {
        return d.GetValue(ViewModelProperty);
    }

    public static void SetViewModel(DependencyObject d, object value)
    {
        d.SetValue(ViewModelProperty, value);
    }
    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d != null && e.NewValue != null)
        {
            BindDataAnotations(d);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static readonly DependencyProperty InputMaskProperty =
        DependencyProperty.RegisterAttached("InputMask", typeof(string), typeof(MVVM), new PropertyMetadata(null, OnInputMaskChanged));

    public static string GetInputMask(DependencyObject d)
    {
        return (string)d.GetValue(InputMaskProperty);
    }

    public static void SetInputMask(DependencyObject d, string value)
    {
        d.SetValue(InputMaskProperty, value);
    }
    private static void OnInputMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            if (d != null && d is TextBox)
            {
                TextBox entry = d as TextBox;

                entry.TextChanged -= OnTextBoxTextChanged;

                if (e.NewValue != null && e.NewValue is string)
                {
                    string Mask = e.NewValue as string;

                    if (!string.IsNullOrEmpty(Mask))
                    {
                        entry.TextChanged += OnTextBoxTextChanged;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.OnInputMaskChanged() : {ex.Message} \r\n");
        }
    }

    // Event OnTextBoxTextChanged used to Mask Input

    private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs args)
    {
        try 
        {
            TextBox entry = sender as TextBox;

            int old_pos = entry.SelectionStart;

            string Mask = MVVM.GetInputMask(entry);

            string text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(Mask))
                return;

            StringBuilder sbtexto = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (i < Mask.Length)
                {
                    char ch = text[i];
                    char chMask = Mask[i];

                    if ((chMask == '9' && char.IsDigit(ch)) ||
                            (chMask == 'A' && char.IsLetter(ch)) ||
                            (chMask == '?' && char.IsLetterOrDigit(ch)) ||
                            ((chMask != '9' && chMask != 'A' && chMask != '?') && chMask == ch)
                        )
                    {
                        sbtexto.Append(ch);
                    }
                    else
                    {
                        // Character nesta posicao esta errado

                        // se mascara nao for 9 A ou ? bota mascara no lugar 
                        // se nao bota place holder

                        if (chMask != '9' && chMask != 'A' && chMask != '?')
                        {
                            sbtexto.Append(chMask);
                        }
                        else
                        {
                            sbtexto.Append(' ');
                        }

                        // se caracter se encaixa na proxima mascara vai usar esse caracter errado na proxima
                        // caso contrario descarta 

                        if (i + 1 < Mask.Length)
                        {
                            char chNextMask = Mask[i + 1];

                            if ((chNextMask == '9' && char.IsDigit(ch)) ||
                                    (chNextMask == 'A' && char.IsLetter(ch)) ||
                                    (chNextMask == '?' && char.IsLetterOrDigit(ch)) ||
                                    ((chNextMask != '9' && chNextMask != 'A' && chNextMask != '?') && chNextMask == ch)
                                )
                            {
                                text = text.Insert(i, " ");
                            }
                        }
                    }
                }
            }

            text = sbtexto.ToString();

            if (entry.Text != text)
            {
                int old_length = entry.Text.Length;

                entry.Text = text;

                int new_length = entry.Text.Length;

                int new_pos = ((old_length == 0 && old_pos == 0) ||
                               (old_length > 0 && old_pos >= old_length - 1)) ? new_length : old_pos;

                entry.Select(new_pos, 0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.OnTextBoxTextChanged() : {ex.Message} \r\n");
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////

    public static readonly DependencyProperty ErrorControlProperty =
        DependencyProperty.RegisterAttached("ErrorControl", typeof(TextBlock), typeof(MVVM), new PropertyMetadata(null, OnErrorControlChanged));

    public static TextBlock GetErrorControl(DependencyObject d)
    {
        return (TextBlock)d.GetValue(ErrorControlProperty);
    }

    public static void SetErrorControl(DependencyObject d, TextBlock value)
    {
        d.SetValue(ErrorControlProperty, value);
    }

    private static void OnErrorControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d != null && e.NewValue != null && e.NewValue is TextBlock)
        {
            string PropertyName = MVVM.GetPropertyName(d);
                      
            object ViewModel = MVVM.GetViewModel(d);

            if (ViewModel != null && !string.IsNullOrWhiteSpace(PropertyName))
            {
                Type type = ViewModel.GetType();

                PropertyInfo prop = type.GetProperty(PropertyName);

                if (prop != null)
                {
                    BindErrorMessage(d, e.NewValue as TextBlock, PropertyName, ViewModel);
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////

    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(MVVM), new PropertyMetadata(string.Empty, OnPropertyNameChanged));

    public static string GetPropertyName(DependencyObject d)
    {
        return (string)d.GetValue(PropertyNameProperty);
    }

    public static void SetPropertyName(DependencyObject d, string value)
    {
        d.SetValue(PropertyNameProperty, value);
    }

    private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d != null && e.NewValue != null && e.NewValue is string && ! string.IsNullOrWhiteSpace (e.NewValue as string))
        {
            BindDataAnotations(d);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private static T FindChildAutoSuggestBox<T>(DependencyObject parent)
    {
#if WINDOWS || WINDOWS_UWP
        try 
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    if (child.GetValue(FrameworkElement.NameProperty).ToString() == "TextBox")
                    {
                        return typedChild;
                    }
                }

                T inner = FindChildAutoSuggestBox<T>(child);

                if (inner != null)
                {
                    return inner;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em MVVM.FindChildAutoSuggestBox() : {ex.Message} \r\n");
        }
#endif
        return default;
    }
}
