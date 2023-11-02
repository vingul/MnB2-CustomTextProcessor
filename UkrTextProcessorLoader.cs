using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Localization;
using TaleWorlds.Library;

namespace CustomTextProcessor
{
    public class UkrTextProcessorLoader : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            /*Type objType = typeof(UkrainianTextProcessor);
            string type_name = objType.AssemblyQualifiedName;
            Debug.Print($"UkrainianTextProcessor.AssemblyQualifiedName: {type_name}");*/

            /*Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Message", new TextObject("Message", null), 9990,
                () => { InformationManager.DisplayMessage(new InformationMessage(type_name)); },
                () => { return (false, null); }));*/
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }
    }
}