using UnityEngine;
using UnityEditor;

public class BatchMaterialImportFix : EditorWindow
{
    [MenuItem("Tools/Batch Fix Material Import Mode")]
    public static void FixMaterialImportMode()
    {
        string[] assetGUIDs = Selection.assetGUIDs;
        foreach (string guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer != null)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None; // Temporarily disable material import
                importer.SaveAndReimport(); // Apply changes
                
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard; // Set to Standard (Legacy)
                importer.SaveAndReimport(); // Apply changes again

                Debug.Log($"Updated Material Creation Mode for: {assetPath}");
            }
        }
    }
}
