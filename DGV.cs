using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rsx.DGV
{



    public class CalendarColumn : DataGridViewColumn
  {
    public CalendarColumn()
        : base(new CalendarCell())
    {
    }

    public override DataGridViewCell CellTemplate
    {
      get
      {
        return base.CellTemplate;
      }
      set
      {
        // Ensure that the cell used for the template is a CalendarCell.
        if (value != null && !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
        {
          throw new InvalidCastException("Must be a CalendarCell");
        }
        base.CellTemplate = value;
      }
    }
  }

  public class CalendarCell : DataGridViewTextBoxCell
  {
    private DateTime minVal = new DateTime(1983, 2, 3, 17, 6, 0, 0, DateTimeKind.Utc);

    public CalendarCell()
        : base()
    {
    }

    public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

      CalendarEditingControl ctl = DataGridView.EditingControl as CalendarEditingControl;
      // Use the default row value when Value property is null.
      DateTime minVal = DateTime.Now;
      DataGridViewColumn col = this.DataGridView.Columns[this.ColumnIndex];
      if (col.Tag != null) minVal = (DateTime)col.Tag;
      if (this.Value == null || Convert.ToString(this.Value).Equals(String.Empty))
      {
        ctl.Value = (DateTime)minVal;
      }
      else
      {
        ctl.Value = (DateTime)this.Value;
      }
    }

    public override Type EditType
    {
      get
      {
        // Return the type of the editing control that CalendarCell uses.
        return typeof(CalendarEditingControl);
      }
    }

    public override Type ValueType
    {
      get
      {
        // Return the type of the value that CalendarCell contains.
        return typeof(DateTime);
      }
    }
  }

  internal class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
  {
    private DataGridView dataGridView;
    private bool valueChanged = false;
    private int rowIndex;

    public CalendarEditingControl()
    {
      this.Format = DateTimePickerFormat.Time;
    }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue
    // property.
    public object EditingControlFormattedValue
    {
      get
      {
        return this.Value.ToString();
      }
      set
      {
        this.Value = DateTime.Parse((String)value);
      }
    }

    // Implements the
    // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
    public object GetEditingControlFormattedValue(
        DataGridViewDataErrorContexts context)
    {
      return EditingControlFormattedValue;
    }

    // Implements the
    // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
    public void ApplyCellStyleToEditingControl(
        DataGridViewCellStyle dataGridViewCellStyle)
    {
      this.Font = dataGridViewCellStyle.Font;
      this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
      this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
    }

    // Implements the IDataGridViewEditingControl.EditingControlRowIndex
    // property.
    public int EditingControlRowIndex
    {
      get
      {
        return rowIndex;
      }
      set
      {
        rowIndex = value;
      }
    }

    // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey
    // method.
    public bool EditingControlWantsInputKey(
        Keys key, bool dataGridViewWantsInputKey)
    {
      // Let the DateTimePicker handle the keys listed.
      switch (key & Keys.KeyCode)
      {
        case Keys.Left:
        case Keys.Up:
        case Keys.Down:
        case Keys.Right:
        case Keys.Home:
        case Keys.End:
        case Keys.PageDown:
        case Keys.PageUp:
          return true;

        default:
          return !dataGridViewWantsInputKey;
      }
    }

    // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit
    // method.
    public void PrepareEditingControlForEdit(bool selectAll)
    {
      // No preparation needs to be done.
    }

    // Implements the IDataGridViewEditingControl
    // .RepositionEditingControlOnValueChange property.
    public bool RepositionEditingControlOnValueChange
    {
      get
      {
        return false;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlDataGridView property.
    public DataGridView EditingControlDataGridView
    {
      get
      {
        return dataGridView;
      }
      set
      {
        dataGridView = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlValueChanged property.
    public bool EditingControlValueChanged
    {
      get
      {
        return valueChanged;
      }
      set
      {
        valueChanged = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingPanelCursor property.
    public Cursor EditingPanelCursor
    {
      get
      {
        return Cursors.Hand;
      }
    }

    protected override void OnValueChanged(EventArgs eventargs)
    {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      valueChanged = true;

      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnValueChanged(eventargs);
    }
  }




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






/*

namespace k0X
{
   public class DataRowColumn : DataGridViewColumn
   {
	  public DataRowColumn()
		 : base(new DataRowCell())
	  {
	  }

	  public override DataGridViewCell CellTemplate
	  {
		 get
		 {
			return base.CellTemplate;
		 }
		 set
		 {
			// Ensure that the cell used for the template is a CalendarCell.
			if (value != null && !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
			{
			   throw new InvalidCastException("Must be a CalendarCell");
			}
			base.CellTemplate = value;
		 }
	  }
   }

   public class DataRowCell : DataGridViewTextBoxCell
   {
	  public DataRowCell()
		 : base()
	  {
	  }

	  public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	  {
		 // Set the value of the editing control to the current cell value.
		 base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

		 DataRowEditingControl ctl = DataGridView.EditingControl as DataRowEditingControl;
		 // Use the default row value when Value property is null.
		 if (this.Value == null || Convert.ToString(this.Value).Equals(String.Empty))
		 {
			//  ctl = this.DataGridView.DataSource.
		 }
		 else
		 {
			ctl.DataSource = (this.Value as System.Data.DataRow).Table;
		 }

		 ctl.Dock = DockStyle.Fill;

		 this.DataGridView.Rows[this.RowIndex].Resizable = DataGridViewTriState.True;
		 this.DataGridView.Rows[this.ColumnIndex].Resizable = DataGridViewTriState.True;
	  }

	  public override Type EditType
	  {
		 get
		 {
			// Return the type of the editing control that CalendarCell uses.
			return typeof(DataRowEditingControl);
		 }
	  }

	  public override Type ValueType
	  {
		 get
		 {
			// Return the type of the value that CalendarCell contains.
			return typeof(System.Data.DataRow);
		 }
	  }
   }

   class DataRowEditingControl : System.Windows.Forms.DataGridView, IDataGridViewEditingControl
   {
	  System.Windows.Forms.DataGridView dataGridView;
	  private bool valueChanged = false;
	  int rowIndex;

	  public DataRowEditingControl()
	  {
	  }

	  // Implements the IDataGridViewEditingControl.EditingControlFormattedValue
	  // property.
	  public object EditingControlFormattedValue
	  {
		 get
		 {
			return this.DataSource.ToString();
		 }
		 set
		 {
			this.DataSource = value;
		 }
	  }

	  // Implements the
	  // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
	  public object GetEditingControlFormattedValue(
		  DataGridViewDataErrorContexts context)
	  {
		 return EditingControlFormattedValue;
	  }

	  // Implements the
	  // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
	  public void ApplyCellStyleToEditingControl(
		  DataGridViewCellStyle dataGridViewCellStyle)
	  {
		 this.Font = dataGridViewCellStyle.Font;
	  }

	  // Implements the IDataGridViewEditingControl.EditingControlRowIndex
	  // property.
	  public int EditingControlRowIndex
	  {
		 get
		 {
			return rowIndex;
		 }
		 set
		 {
			rowIndex = value;
		 }
	  }

	  // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey
	  // method.
	  public bool EditingControlWantsInputKey(
		  Keys key, bool dataGridViewWantsInputKey)
	  {
		 // Let the DateTimePicker handle the keys listed.
		 switch (key & Keys.KeyCode)
		 {
			case Keys.Left:
			case Keys.Up:
			case Keys.Down:
			case Keys.Right:
			case Keys.Home:
			case Keys.End:
			case Keys.PageDown:
			case Keys.PageUp:
			return true;

			default:
			return !dataGridViewWantsInputKey;
		 }
	  }

	  // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit
	  // method.
	  public void PrepareEditingControlForEdit(bool selectAll)
	  {
		 // No preparation needs to be done.
	  }

	  // Implements the IDataGridViewEditingControl
	  // .RepositionEditingControlOnValueChange property.
	  public bool RepositionEditingControlOnValueChange
	  {
		 get
		 {
			return false;
		 }
	  }

	  // Implements the IDataGridViewEditingControl
	  // .EditingControlDataGridView property.
	  public DataGridView EditingControlDataGridView
	  {
		 get
		 {
			return dataGridView;
		 }
		 set
		 {
			dataGridView = value;
		 }
	  }

	  // Implements the IDataGridViewEditingControl
	  // .EditingControlValueChanged property.
	  public bool EditingControlValueChanged
	  {
		 get
		 {
			return valueChanged;
		 }
		 set
		 {
			valueChanged = value;
		 }
	  }

	  // Implements the IDataGridViewEditingControl
	  // .EditingPanelCursor property.
	  public Cursor EditingPanelCursor
	  {
		 get
		 {
			return Cursors.Hand;
		 }
	  }
   }
}

*/
