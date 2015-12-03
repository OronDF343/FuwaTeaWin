Creating Skins
--------------

Skins can be written in a single XAML file, or a compiled DLL file.
See FTWPlayer\Skins\Default\*.xaml for all the resources you can override and more examples/information.
You must give each file the same name as the one containing the resources you want to override.
Place all your XAML files in a subfolder in %install-directory%\skins or in %userdata-directory%\skins.
Each XAML file in the skin should have the following structure:

    <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:skins="clr-namespace:FTWPlayer.Skins;assembly=FTWPlayer">
        <skins:ResourceDictionaryIdentifier x:Key="SkinIdentifier"
                                            Id="OronDF343:Glacier"
                                            Name="Ice -Glacier-"
                                            Description="Adds more color to the default skin!"
                                            Version="1.0"
                                            Author="OronDF343"
                                            Homepage="https://orondf343.wordpress.com/"
                                            Parent="Default" />
    
        <!-- Your resources here -->
		
    </ResourceDictionary>

* All of the XML namespaces must be specified, and they must be assembly-qualified. This means that for example "clr-namespace:FTWPlayer.Skins" is not enough, you need it to be "clr-namespace:FTWPlayer.Skins;assembly=FTWPlayer".
* Each skin should have a ResourceDictionaryIdentifier with x:Key="SkinIdentifier". These are the properties it has:
* Id (string): A unique ID for your skin. It is recommended to use the "Author:Skin" format.
* Name (string): The name that will be displayed for the skin.
* Description (string): The description that will be displayed for the skin.
* Version (string): The version that will be displayed for the skin.
* Author (string): The author that will be displayed for the skin.
* Homepage (string): The info link that will be displayed for the skin.
* Parent (string): The ID of another skin that this one depends on.
* Localization is not supported for XAML files unless the string exists in FTWPlayer.exe.
* Use compiled DLL skins if you want more features (this option is probably bugged, you can alternatively create an extension that loads your skin correctly).
