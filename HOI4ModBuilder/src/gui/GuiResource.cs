using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.gui
{
    partial class GuiResource : UserControl
    {
        private int index;

        public GuiResource()
        {
            InitializeComponent();
        }
        public GuiResource(int index)
        {
            InitializeComponent();
            this.index = index;
        }

        public void LoadFromResource(Resource resource)
        {
            textBox_Tag.Text = resource.tag;
            comboBox_IconFrame.Text = resource.iconFrame.ToString();
            textBox_FactoryPerUnit.Text = resource.factoryCostPerUnit.ToString();
            textBox_ConvoysPerUnit.Text = resource.convoysPerUnit.ToString();
        }

        public void SaveToResource(Resource resource)
        {
            try
            {
                resource.tag = textBox_Tag.Text;
                resource.iconFrame = uint.Parse(comboBox_IconFrame.Text);
                resource.factoryCostPerUnit = float.Parse(textBox_FactoryPerUnit.Text);
                resource.convoysPerUnit = float.Parse(textBox_ConvoysPerUnit.Text);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void button_AddAfter_Click(object sender, EventArgs e)
        {

        }

        private void button_Remove_Click(object sender, EventArgs e)
        {

        }

        private void button_MoveUp_Click(object sender, EventArgs e)
        {

        }

        private void button_MoveDown_Click(object sender, EventArgs e)
        {

        }
    }
}
