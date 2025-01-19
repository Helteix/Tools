using System;
using System.Linq;
using System.Reflection;
using LTX.Tools.SerializedComponent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.SerializedComponent
{
    public class AddComponentDropdown
    {
        public event Action<Type> OnTypeSelected;

        private readonly Type typeConstraint;
        private readonly string pathConstraint;
        private GenericMenu menu;

        public AddComponentDropdown(string pathConstraint = null, Type typeConstraint = null, bool showNonCompatibleComponents = false) : base()
        {
            this.typeConstraint = typeConstraint;
            this.pathConstraint = pathConstraint;
            menu = new GenericMenu();

            Type t = typeConstraint ?? typeof(ISComponent);

            var types = TypeCache.GetTypesDerivedFrom(typeof(ISComponent));

            foreach (Type type in types)
            {
                if(type.IsAbstract || type.IsInterface)
                    continue;

                if(type.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;

                string path = $"Others/{type.Name}";
                var attribute = type.GetCustomAttribute<AddSerializedComponentMenuAttribute>();
                if (attribute != null)
                    path = attribute.Path;

                bool valid = true;

                if(!string.IsNullOrEmpty(pathConstraint))
                    valid = !path.Contains(pathConstraint);

                if (t != type && ((t.IsClass && !type.IsSubclassOf(t)) || (type.GetInterfaces().All(ctx => ctx != t))))
                    valid = false;

                if(valid)
                    menu.AddItem(new GUIContent(path), false, () => { OnTypeSelected?.Invoke(type); });
                else if(showNonCompatibleComponents)
                    menu.AddDisabledItem(new GUIContent(path), false);
            }
        }

        public void Show(EventBase eventBase)
        {
            menu.DropDown(new Rect(eventBase.originalMousePosition, Vector2.zero));
        }
    }
}