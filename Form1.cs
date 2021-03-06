﻿//new
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderGrid
{
    public partial class Form1 : Form
    {
        DataTable dataTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.initDataGridView();
        }

        private void initDataGridView()
        {
            dataTable = GetTable();

            this.dataGridViewTestTable.DataSource = null;
            this.dataGridViewTestTable.DataSource = dataTable;
            this.dataGridViewTestTable.Columns["CurrentOrder"].Visible = false;
            this.dataGridViewTestTable.Sort(this.dataGridViewTestTable.Columns["CurrentOrder"], ListSortDirection.Ascending);
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            List<int> listSelectedValuesId = new List<int>();
            int currentValueId = (int)this.dataGridViewTestTable.CurrentRow.Cells["Id"].Value;

            //in order to get the value for indexRowLast right we temporarly disable adding right on grid; otherwise index would be +1 
            bool allowUserToAddRows = this.dataGridViewTestTable.AllowUserToAddRows;
            this.dataGridViewTestTable.AllowUserToAddRows = false;

            int indexRowFirst = this.dataGridViewTestTable.Rows.GetFirstRow(DataGridViewElementStates.Visible);
            int indexRowFirstSelected = this.dataGridViewTestTable.Rows.GetFirstRow(DataGridViewElementStates.Selected);

            this.dataGridViewTestTable.AllowUserToAddRows = allowUserToAddRows;

            //run it only if among the selected rows is not the first row 
            if (indexRowFirst != indexRowFirstSelected)
            {
                //important! pick the selected records in the current ascending order
                List<DataGridViewRow> selectedRows = (List<DataGridViewRow>)this.dataGridViewTestTable.SelectedRows
                                                                    .Cast<DataGridViewRow>()
                                                                    .OrderBy(r => r.Index)
                                                                    .ToList<DataGridViewRow>();

                foreach (DataGridViewRow selectedRow in selectedRows.ToArray())
                {
                    listSelectedValuesId.Add((int)selectedRow.Cells["Id"].Value);
                }

                foreach (int selectedValueId in listSelectedValuesId.ToArray())
                {
                    if (selectedValueId != 0)
                    {
                        //-1 means ascending
                        this.moveUpDown(selectedValueId, -1);
                    }
                }

                if (selectedRows.Count > 0)
                {
                    this.positionCursorOnSelected(listSelectedValuesId, currentValueId);
                }
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            List<int> listSelectedValuesId = new List<int>();
            int currentValueId = (int)this.dataGridViewTestTable.CurrentRow.Cells["Id"].Value;

            //in order to get the value for indexRowLast right we temporarly disable adding right on grid; otherwise index would be +1 
            bool allowUserToAddRows = this.dataGridViewTestTable.AllowUserToAddRows;
            this.dataGridViewTestTable.AllowUserToAddRows = false;

            int indexRowLast = this.dataGridViewTestTable.Rows.GetLastRow(DataGridViewElementStates.Visible);
            int indexRowLastSelected = this.dataGridViewTestTable.Rows.GetLastRow(DataGridViewElementStates.Selected);

            this.dataGridViewTestTable.AllowUserToAddRows = allowUserToAddRows;

            //run it only if among the selected rows is not the last row 
            if (indexRowLast != indexRowLastSelected)
            {
                //important! pick the selected records in the current descending order
                List<DataGridViewRow> selectedRows = (List<DataGridViewRow>)this.dataGridViewTestTable.SelectedRows
                                                                    .Cast<DataGridViewRow>()
                                                                    .OrderByDescending(r => r.Index)
                                                                    .ToList<DataGridViewRow>();

                foreach (DataGridViewRow selectedRow in selectedRows.ToArray())
                {
                    listSelectedValuesId.Add((int)selectedRow.Cells["Id"].Value);
                }

                foreach (int selectedValueId in listSelectedValuesId.ToArray())
                {
                    if (selectedValueId != 0)
                    {
                        //+1 means descending
                        this.moveUpDown(selectedValueId, 1);
                    }
                }

                if (selectedRows.Count > 0)
                {

                    this.positionCursorOnSelected(listSelectedValuesId, currentValueId);
                }
            }
        }

        private void moveUpDown(int idToMove, int posToMove) //-1 = one up, 1 = one down on the list
        {
            //update dataTable
            DataRow rowToMove = dataTable.Rows
                        .Cast<DataRow>()
                        .Where(r => (int)r["Id"] == idToMove)
                        .OrderBy(r => r["Id"])
                        .FirstOrDefault();



            if (rowToMove != null)
            {
                //select and update the previous or next row
                DataRow row = null;
                if (posToMove < 0)
                {
                    row = dataTable.Rows
                        .Cast<DataRow>()
                        .Where(r => (int)r["CurrentOrder"] < (int)rowToMove["CurrentOrder"])
                        .OrderByDescending(r => r["CurrentOrder"])
                        .FirstOrDefault();
                }
                else
                {
                    if (posToMove > 0)
                    {

                        row = dataTable.Rows
                            .Cast<DataRow>()








                            .Where(r => (int)r["CurrentOrder"] > (int)rowToMove["CurrentOrder"])
                            .OrderBy(r => r["CurrentOrder"])
                            .FirstOrDefault();
                    }

                }

                if (row != null)
                {
                    row.BeginEdit();
                    row["CurrentOrder"] = -999;
                    row.EndEdit();
                    row.AcceptChanges();

                    //move the current row up/down
                    rowToMove.BeginEdit();
                    rowToMove["CurrentOrder"] = (int)rowToMove["CurrentOrder"] + posToMove;
                    rowToMove.EndEdit();
                    rowToMove.AcceptChanges();

                    row.BeginEdit();
                    row["CurrentOrder"] = (int)rowToMove["CurrentOrder"] - posToMove;
                    row.EndEdit();
                    row.AcceptChanges();
                }
            }
        }

        private void positionCursorOnSelected(List<int> listIdValue, int currentValueId)
        {
            bool valueAllowUserToAddRows = this.dataGridViewTestTable.AllowUserToAddRows;

            //temporary disable it
            this.dataGridViewTestTable.AllowUserToAddRows = false;
            this.dataGridViewTestTable.ClearSelection();

            DataGridViewRow rowCurrentValueId = this.dataGridViewTestTable.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["Id"].Value.Equals(currentValueId))
                .OrderByDescending(r => r.Cells["Id"])
                .FirstOrDefault();

            this.dataGridViewTestTable.CurrentCell = rowCurrentValueId.Cells[1];

            foreach (int idValue in listIdValue.ToArray())
            {
                DataGridViewRow row = this.dataGridViewTestTable.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Cells["Id"].Value.Equals(idValue))
                    .OrderByDescending(r => r.Cells["Id"])
                    .FirstOrDefault();

                if (row != null)
                {

                    this.dataGridViewTestTable.Rows[row.Index].Selected = true;
                }
            }

            this.dataGridViewTestTable.AllowUserToAddRows = valueAllowUserToAddRows;
        }

        static DataTable GetTable()
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("CurrentOrder", typeof(int));

            // Here we add five DataRows.
            table.Rows.Add(10, "Hydralazine", 1);
            table.Rows.Add(21, "Combivent", 2);
            table.Rows.Add(25, "Indocin", 3);
            table.Rows.Add(50, "Enebrel", 4);
            table.Rows.Add(100, "Dilantin", 5);

            return table;

        }


        private void dataGridViewTestTable_SelectionChanged(object sender, EventArgs e)
        {
            richTextBoxDetailsGrid.Clear();

            richTextBoxDetailsGrid.AppendText("Current row details:");

            if (dataGridViewTestTable != null
                && dataGridViewTestTable.CurrentRow != null)
                richTextBoxDetailsGrid.AppendText(String.Format("Index: {0}, Id: {1}", dataGridViewTestTable.CurrentRow.Index, dataGridViewTestTable.CurrentRow.Cells["Id"].Value));


        }

    }
}
