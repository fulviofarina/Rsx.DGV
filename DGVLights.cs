using System.Drawing;
using System.Windows.Forms;

namespace Rsx.DGV
{
    /// <summary>
    /// The kind of cell with bool value that can be red = false or green = true
    /// </summary>
    public partial class DGVLightsCell : DataGridViewTextBoxCell
    {
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if ((bool)value)
            {
                cellStyle.BackColor = Color.Red;
            }
            else cellStyle.BackColor = Color.DarkGreen;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
    }
    /// <summary>
    /// The kind of column with bool value that can be red = false or green = true
    /// </summary>
    public partial class DGVLightsColumn : DataGridViewTextBoxColumn
    {
        public DGVLightsColumn()
            : base()
        {
            base.CellTemplate = new DGVLightsCell();
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }

            set
            {
                base.CellTemplate = value;
            }
        }
    }
}