using System.Text.RegularExpressions;
using UnityEditor;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class ClearAllNoneElementContextualPropertyMenu
    {
        static ClearAllNoneElementContextualPropertyMenu()
        {
            EditorApplication.contextualPropertyMenu -= OnContextualPropertyMenu;
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
        }

        private static void OnContextualPropertyMenu( GenericMenu menu, SerializedProperty property )
        {
            if ( !property.isArray ) return;
            if ( !Regex.IsMatch( property.arrayElementType, @"PPtr<\$(.*?)>" ) ) return;

            var copiedProperty = property.Copy();

            menu.AddItem
            (
                content: new( "Clear All None Element" ),
                on: false,
                func: () =>
                {
                    var isChanged = false;

                    for ( var i = copiedProperty.arraySize - 1; 0 <= i; i-- )
                    {
                        var element = copiedProperty.GetArrayElementAtIndex( i );
                        var value   = element.objectReferenceValue;

                        if ( value != null ) continue;

                        copiedProperty.DeleteArrayElementAtIndex( i );

                        isChanged = true;
                    }

                    if ( !isChanged ) return;

                    copiedProperty.serializedObject.ApplyModifiedProperties();
                }
            );
        }
    }
}