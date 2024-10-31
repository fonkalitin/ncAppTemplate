
using HostMgd.EditorInput;
using Application = HostMgd.ApplicationServices.Application;
using HostMgd.ApplicationServices;
using Multicad.Runtime;
using Teigha.DatabaseServices;

namespace ncAppTemplate
{
    public class Class1
    {
        [CommandMethod("test-selection", CommandFlags.UsePickSet | CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void TestSelectionCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;

            Editor ed = doc.Editor;

            PromptSelectionResult selResult = ed.SelectImplied();
            if (selResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nNo selected entities");
                return;
            }

            SelectionSet selValue = selResult.Value;
            ObjectId[] ids = selResult.Value.GetObjectIds();
        }
    }
}