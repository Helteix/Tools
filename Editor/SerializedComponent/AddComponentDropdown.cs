using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.SerializedComponent
{
    public class AddComponentDropdown : DropdownMenu
    {
        public event Action<SerializedComponentLibrary.TypeInfos> OnTypeSelected;

        private readonly Type typeConstraint;
        private readonly string pathConstraint;


        public AddComponentDropdown(VisualElement visualElement, string pathConstraint = null, Type typeConstraint = null) : base()
        {
            this.typeConstraint = typeConstraint;
            this.pathConstraint = pathConstraint;

            var types = SerializedComponentLibrary.GetTypes();
            visualElement.AddManipulator(new ContextualMenuManipulator(ctx =>
            {
                foreach (SerializedComponentLibrary.TypeInfos type in types)
                {
                    AppendAction(type.path,
                        action =>
                        {
                            OnTypeSelected?.Invoke(type);
                        },
                        DropdownMenuAction.Status.Normal);
                }
            }));
            foreach (SerializedComponentLibrary.TypeInfos type in types)
            {
                AppendAction(type.path,
                    action =>
                    {
                        OnTypeSelected?.Invoke(type);
                    },
                    DropdownMenuAction.Status.Normal);
            }
        }

/*
        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem(pathConstraint);

            Dictionary<string, AdvancedDropdownItem> items = new()
            {
                { pathConstraint, root }
            };

            var types = SerializedComponentLibrary.GetTypes();

            foreach (SerializedComponentLibrary.TypeInfos type in types)
            {
                AdvancedDropdownItem bindedElement = GetElementOrCreateIt(items, type.path);
                callbacks.Add(bindedElement.name, () => OnTypeSelected?.Invoke(type));
            }

            return root;
        }

        private AdvancedDropdownItem GetElementOrCreateIt(
            Dictionary<string, AdvancedDropdownItem> items,
            string path)
        {
            if (items.TryGetValue(path, out var item))
                return item;

            string[] sections = path.Split('/');
            string name = string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < sections.Length - 1; i++)
            {
                name = sections[i];
                builder.Append(name);
            }

            string rootPath = builder.ToString();
            if (rootPath == pathConstraint)
            {
                AdvancedDropdownItem mainElement = new (string.Empty);
                items.Add(pathConstraint, mainElement);
                return mainElement;
            }

            AdvancedDropdownItem root = GetElementOrCreateIt(items, rootPath);
            AdvancedDropdownItem newItem = new AdvancedDropdownItem(name);
            root.AddChild(newItem);

            return newItem;
        }*/
    }
}