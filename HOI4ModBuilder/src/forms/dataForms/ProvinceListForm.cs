using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.forms
{
    partial class ProvinceListForm : Form
    {
        public static ProvinceListForm instance;
        public Province selectedProvince;

        public ProvinceListForm()
        {
            InitializeComponent();
            instance?.Invoke((MethodInvoker)delegate { instance?.Close(); });
            instance = this;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadData();
            GuiLocManager.formsReinitEvents.Add(this, () =>
            {
                Invoke((MethodInvoker)delegate
                {
                    Controls.Clear();
                    InitializeComponent();
                    LoadData();
                });
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
        {
            DataGridView data = instance.DataGridView_Provinces;
            data.Rows.Clear();

            Task<DataGridViewRow[]> assembleTask = new Task<DataGridViewRow[]>(
                () =>
                {
                    ushort id;
                    ushort[] ids = ProvinceManager.GetProvincesIds().OrderBy(x => x).ToArray();

                    DataGridViewRow[] rows = new DataGridViewRow[ids.Length];

                    DataGridViewRow row;

                    for (int i = 0; i < ids.Length; i++)
                    {
                        id = ids[i];
                        ProvinceManager.TryGetProvince(id, out Province p);
                        int color = p.Color;

                        row = new DataGridViewRow();
                        row.CreateCells(data, new object[] {
                            id,
                            (byte)(color >> 16),
                            (byte)(color >> 8),
                            (byte)color,
                            p.GetTypeString(),
                            p.IsCoastal,
                            p.Terrain == null ? "unknown" : p.Terrain.name,
                            p.ContinentId,
                            p.State == null ? -1 : p.State.Id,
                            p.Region == null ? -1 : p.Region.Id,
                            p.pixelsCount,
                            p.borders.Count
                        });
                        rows[i] = row;
                    }

                    return rows;
                }
            );

            assembleTask.ContinueWith(
                task => data.Rows.AddRange(task.Result),
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            assembleTask.Start();
        }


        private void Button_Find_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                ushort id = ushort.Parse(TextBox_Id.Text);
                if (ProvinceManager.TryGetProvince(id, out selectedProvince))
                {
                    SetColor(Color.FromArgb(selectedProvince.Color));
                }
                else
                {
                    Panel_Color.BackColor = Color.White;
                    TextBox_Red.Text = "";
                    TextBox_Green.Text = "";
                    TextBox_Blue.Text = "";
                }
            });
        }

        private void SetColor(Color color)
        {
            Panel_Color.BackColor = color;
            TextBox_Red.Text = "" + color.R;
            TextBox_Green.Text = "" + color.G;
            TextBox_Blue.Text = "" + color.B;
        }

        private void Button_SetFirstColor_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => MainForm.Instance.SetBrushFirstColor(Panel_Color.BackColor));
        }

        private void Button_SetSecondColor_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => MainForm.Instance.SetBrushSecondColor(Panel_Color.BackColor));
        }

        private void Button_GenerateColor_Random_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => SetColor(Color3B.GetRandowColor().ToColor()));
        }

        private void Button_GenerateColor_Land_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => SetColor(ProvinceManager.GetNewLandColor().ToColor()));
        }

        private void Button_GenerateColor_Sea_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => SetColor(ProvinceManager.GetNewSeaColor().ToColor()));
        }

        private void Button_GenerateColor_Lake_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => SetColor(ProvinceManager.GetNewLakeColor().ToColor()));
        }
    }
}
