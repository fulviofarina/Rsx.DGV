using System;
using System.Data;
using System.Windows.Forms;

namespace Rsx.DGV
{
    public interface IDGVControlCreate
    {
        DataGridView[] DGVs { get; set; }
      
        object UsrControl { get; set; }

   
        void CreateButtonEvents(ref ToolStripButton[] items);
        bool CreateDGVEvents();
      //  bool CreateDGVEvents(ref DataGridView[] dgvs);
       
      
        void SetContext(string controlHeader, ref DataGridView[] dgvs, ContextMenuStrip cms);
      
    }
}