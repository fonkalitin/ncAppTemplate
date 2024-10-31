
using HostMgd.EditorInput;
using HostMgd.ApplicationServices;
using Teigha.DatabaseServices;
using Db = Teigha.DatabaseServices;
using Teigha.Runtime;
using Multicad.Runtime;
using Multicad;
using Multicad.DatabaseServices;
using System.Security.Cryptography;
using Multicad.Objects;

namespace ncAppTemplate
{
    public class Class1
    {
        [Teigha.Runtime.CommandMethod("test-selection", Teigha.Runtime.CommandFlags.UsePickSet)]
        public static void TestSelectionCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;

            Editor ed = doc.Editor;


            PromptSelectionResult selResult = ed.SelectImplied();
            if (selResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nНе выбрано ни одного обьекта");
                return;
            }
            
            SelectionSet selValue = selResult.Value;

            SelectedObject en = selValue[0];
            
            ObjectId[] ids = selResult.Value.GetObjectIds();
            
            McObjectId mcsId = McObjectId.FromOldIdPtr(ids[0].OldIdPtr);
            McObject currParentObj = mcsId.GetObject();

            if (currParentObj is McParametricObject currParParentObj)
            {
                McDbEntity HighlightenObj = currParParentObj.DbEntity;
                HighlightenObj.Highlight(true);
            }

            ed.WriteMessage($"\nВ наборе обнаружено объектов: {ids.Length}");
            ed.GetString($"\nПодсветка зеленым");

        }
    }
}