
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
using Multicad.Symbols;
using App = HostMgd.ApplicationServices;

namespace ncAppTemplate
{
    public class Class1
    {
        [Teigha.Runtime.CommandMethod("test-selection", Teigha.Runtime.CommandFlags.UsePickSet)]
        public static void TestSelectionCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) { return; }

            Editor ed = doc.Editor;
            PromptSelectionResult selResult = ed.SelectImplied();
            if (selResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nНе выбрано ни одного обьекта");
                return;
            }

            SelectionSet selValue = selResult.Value;
            ObjectId[] DbIds = selResult.Value.GetObjectIds();
            ed.WriteMessage($"\nВ наборе обнаружено объектов: {DbIds.Length}");

            foreach (ObjectId DbId in DbIds)
            {

                McObjectId mcsId = McObjectId.FromOldIdPtr(DbId.OldIdPtr); // Преобразование ObjectId >>> McObjectId
                McObject currParentObj = mcsId.GetObject();


                if (currParentObj is McParametricObject currParParentObj)
                {
                    McDbEntity HighlightenObj = currParParentObj.DbEntity;
                    HighlightenObj.Highlight(true);
                }
            }
            ed.GetString($"\nПодсветка зеленым");
        }


        [Teigha.Runtime.CommandMethod("testCmd", Teigha.Runtime.CommandFlags.Session)]
        public void ErrIDOldIdPtrUsing()
        {
            App.Document doc = App.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            List<McNote> lFrmts1 = new List<McNote>();
            List<McNote> lFrmts2 = new List<McNote>();
            List<McNote> lFrmts3 = new List<McNote>();

            Database dbOld = HostApplicationServices.WorkingDatabase;

            string sFilPath = @"c:\Temp\test.dwg";

            int iCount = 100;
            int iScip1 = 0;
            int iScip2 = 0;
            int iScip3 = 0;
            for (int i = 0; i < iCount; i++)
            {

                List<ObjectId> lObId = new List<ObjectId>();
                using (Database db = new Database(false, true))
                {
                    db.ReadDwgFile(sFilPath, FileOpenMode.OpenForReadAndAllShare, false, "");

                    db.CloseInput(true);
                    HostApplicationServices.WorkingDatabase = db;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = tr.GetObject(db.BlockTableId,
                                            OpenMode.ForRead) as BlockTable;
                        foreach (ObjectId btrId in bt)
                        {
                            BlockTableRecord btr = tr.GetObject(btrId,
                                                        OpenMode.ForRead) as BlockTableRecord;
                            if (!btr.IsFromExternalReference /*не ссылка*/
                                && !btr.IsDependent /*не зависимый*/
                                && btr.IsLayout /*пространство*/)
                            {
                                //!получаем запись BlockTableRecord пространства
                                BlockTableRecord btrs = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                                foreach (ObjectId brId in btrs)//в пространстве перебираем все что в нем находится
                                {
                                    //!по РХ выбираем выноски
                                    if (brId.ObjectClass.Name == "mcsDbObjectNote")
                                    {
                                        lObId.Add(brId);
                                        //! в транзакции по OldIdPtr
                                        McObjectId mcsId = McObjectId.FromOldIdPtr(brId.OldIdPtr);
                                        McNote mcFormat = mcsId.GetObject()?.Cast<McNote>();
                                        if (mcFormat == null)
                                        {
                                            iScip1++;//в mcsId черт знает что как на картинке выше
                                        }
                                        else//теряет выноски
                                        {
                                            lFrmts1.Add(mcFormat);
                                        }
                                    }
                                }
                            }
                        }
                        tr.Commit();
                    }

                    //!вне транзакции по OldIdPtr
                    foreach (ObjectId br in lObId)
                    {

                        McObjectId mcsId = Multicad.McObjectId.FromOldIdPtr(br.OldIdPtr);
                        McNote mcFormat = mcsId.GetObject()?.Cast<McNote>();
                        if (mcFormat == null)
                        {
                            iScip2++;
                        }
                        else//теряет выноски
                        {
                            lFrmts2.Add(mcFormat);
                        }
                    }
                }
            }


        }
    }
}