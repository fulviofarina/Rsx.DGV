using Rsx.Dumb;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.DGV
{


    public class CalculableColumn : DataGridViewTextBoxColumn
    {

        public CalculableColumn()
            : base()
        {
            CellTemplate = new CalculableCell();
        }
    }
    public class CalculableCell : DataGridViewTextBoxCell
    {
        //  protected internal Color defaultColor = Color.DarkRed;


        protected override void OnMouseDoubleClick (DataGridViewCellMouseEventArgs e)
        {
            DataRow u = ((this.OwningRow?.DataBoundItem as DataRowView)?.Row) as DataRow;
            if (!EC.IsNuDelDetch(u))
            {
                ICalculableRow Icalc = u as ICalculableRow;
           
                Icalc.ToDo = !Icalc.ToDo;
               
                base.DataGridView.NotifyCurrentCellDirty(true);
                base.DataGridView.ClearSelection();
            }
            base.OnMouseDoubleClick(e);
        }


        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            bool changed = setCellColor();
            //   this.Style
         //   if (changed)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, this.Style, advancedBorderStyle, paintParts);
            }
        }


        protected internal bool setCellColor()
        {
            //   UnitRow u = ((this.OwningRow?.DataBoundItem as DataRowView)?.Row) as UnitRow;
            DataRow u = ((this.OwningRow?.DataBoundItem as DataRowView)?.Row) as DataRow;
            bool changed = false;
            if (!EC.IsNuDelDetch(u))
            {
                //    CalculableColumn c = (CalculableColumn)this.OwningColumn;
                //   bool todo = u.Field<bool>(c.BindingColumn);
                ICalculableRow Icalc = u as ICalculableRow;
                Color colr = Color.DarkGreen;
              
                    if (Icalc.ToDo && Icalc.IsBusy)
                    {
                        colr = Color.DarkOrange;
                    }
                    else if (Icalc.ToDo && !Icalc.IsBusy)
                    {
                        colr = Color.DarkRed;
                    }
                if (colr != Style.BackColor)
                {
                    Style.BackColor = colr;
                    Style.ForeColor = colr;
                    changed = true;

                }
            }
            return changed;
        }

        public CalculableCell() : base()
        {
            Style.SelectionBackColor = Color.Transparent;
            Style.SelectionForeColor = Color.Transparent;

            Color defaultColor = Color.DarkRed;
            Style.BackColor = defaultColor;
            Style.ForeColor = defaultColor;
        }
    }




    public interface IBindableDGVColumn
    {
        DefaultAction DefaultAction { set; }

        string BindingPreferenceField { get; set; }
        DataRow BindingPreferenceRow { get; set; }

        void SetAction();
    }

    public enum DefaultAction
    {
        ReadOnly,
        Visibility,
        Enable
    };
    public partial class BindableDGVCell : DataGridViewTextBoxCell
    {

        protected internal BindableDGVColumn parent
        {
            get
            {
                return this.OwningColumn as BindableDGVColumn;
            }
        }

        /// <summary>
        /// Sets the Style
        /// </summary>
        /// <param name="readonlyMode"></param>
        protected internal void setCellReadOnlyOrNormal()
        {
            if (parent.ReadOnly)
            {
                Style.BackColor = parent.arrayOfBackColors[0];
                this.Style.ForeColor = parent.arrayOfForeColors[0];
                this.Style.SelectionBackColor = parent.arrayOfBackColors[0];
                this.Style.SelectionForeColor = parent.arrayOfForeColors[0];
            }
            else
            {
                Style.BackColor = parent.arrayOfBackColors[2];
                Style.ForeColor = parent.arrayOfForeColors[2];
                Style.SelectionBackColor = parent.arrayOfBackColors[1];
                Style.SelectionForeColor = parent.arrayOfForeColors[1];
            }

        }

        /// <summary>
        /// Sets if should be read only or not
        /// </summary>
        /*
        protected internal void setReadOnlyOrNot()
        {
            if (!string.IsNullOrEmpty(parent?.BindingPreferenceField))
            {
                bool Isreadonly = false;
                Isreadonly = (bool)parent?.BindingPreferenceRow[parent?.BindingPreferenceField];
                setCellReadOnlyOrNormal(Isreadonly);
                if (Isreadonly != this.OwningColumn.ReadOnly)
                {
                    this.OwningColumn.ReadOnly = Isreadonly;
                }
            }
        }
        */
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {

            parent.SetAction();

            if (parent.DefaultAction == DefaultAction.ReadOnly)
            {
             //   parent.SetAction();
                setCellReadOnlyOrNormal();
            }

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        public BindableDGVCell() : base()
        {
            Style.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
        }
    }

    public class BindableDGVColumn : DataGridViewTextBoxColumn, IBindableDGVColumn
    {
        //default colors
        public Color[] arrayOfBackColors = new Color[] { Color.FromArgb(64, 64, 64), Color.White, Color.White };
        public Color[] arrayOfForeColors = new Color[] { Color.Yellow, Color.Black, Color.Black };

        public BindableDGVColumn() : base()
        {
           
        }

        protected internal DefaultAction defaultAction = DefaultAction.ReadOnly;

        protected internal string bindingField = string.Empty;

        protected internal DataRow bindingRow = null;

        public string BindingPreferenceField
        {
            get
            {
                return bindingField;
            }

            set
            {
                bindingField = value;
            }
        }

        public DataRow BindingPreferenceRow
        {
            get
            {
                return bindingRow;
            }

            set
            {
                bindingRow = value;

                SetAction();

            }
        }

        public void SetAction()
        {
           
            if (defaultAction == DefaultAction.ReadOnly)
            {
                SetReadOnly();
            }
            else if (defaultAction == DefaultAction.Visibility)
            {
                SetVisibility();
            }
            else if (defaultAction == DefaultAction.Enable)
            {
                SetEnable();
            }
         //   SetRounding();

        }

        public void SetEnable()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? Isreadonly = (bool?)bindingRow?[bindingField];
            if (Isreadonly != null) this.ReadOnly = (bool)!Isreadonly;

        }

        protected internal string bindingRoundingField = string.Empty;
        public void SetRounding()
        {

            try
            {
                if (!string.IsNullOrEmpty(bindingRoundingField))
                {
                    string rounding = (string)bindingRow?.Field<string>(bindingRoundingField);

                    if (rounding.Count() == 2)
                    {
                        char first = rounding.ToUpper()[0];
                        char[] formatchars = { 'G', 'F', 'N', 'E', 'C' };
                        if (formatchars.Contains(first))
                        {
                            char second = rounding[1];
                            if (char.IsNumber(second))
                            {
                                this.DefaultCellStyle.Format = rounding;
                            }
                        }
                    }
                    this.DataGridView.InvalidateColumn(this.Index);
                }
            }
            catch (FormatException)
            {

            }

        }

        public string BindingRoundingField
        {
            get
            {
                return bindingRoundingField;
            }
            set
            {
                bindingRoundingField = value;
            }
        }

        public DefaultAction DefaultAction
        {

            get
            {
                return defaultAction;
            }
            set
            {
                defaultAction = value;
            }
        }

        public void SetReadOnly()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? Isreadonly = (bool?)bindingRow?[bindingField];
            if (Isreadonly != null) this.ReadOnly = (bool)Isreadonly;

        }
        public void SetVisibility()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? isvisible = (bool?)bindingRow?[bindingField];
            if (isvisible != null) this.Visible = (bool)isvisible;
           
          
        }
    }


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
