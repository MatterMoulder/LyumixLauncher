using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LyumixLauncher
{
    public partial class ModuleSettings : Form
    {
        private int location = 0;
        private List<PropertyItem> _properties;
        private string _moduleName;

        public ModuleSettings(List<PropertyItem> properties, string moduleName, string moduleDisplayName, int moduleVersion)
        {
            _properties = properties;
            _moduleName = moduleName;
            this.Text = moduleDisplayName + " (v" + moduleVersion + ") Settings"; // e.g. "Module Settings"
            this.Name = moduleDisplayName + " (v" + moduleVersion + ") Settings"; // e.g. "Module Settings"
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            InitializeComponent();
            InitializeControls();
            this.AutoScroll = true;
        }

        private void InitializeControls()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            foreach (var property in _properties)
            {
                var label = new Label
                {
                    Text = property.Display,
                    Width = 300
                };
                panel.Controls.Add(label);

                switch (property.Type)
                {
                    case "ComboBox":
                        ComboBox comboBox = new ComboBox
                        {
                            Name = property.Name,
                            Width = 300,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            //DataSource = JsonConvert.DeserializeObject<string[]>(property.Options)
                        };
                        foreach (var option in JsonConvert.DeserializeObject<string[]>(property.Options))
                        {
                            if (option == property.Value.ToString())
                            {
                                int index = comboBox.Items.Add(option);
                                comboBox.SelectedIndex = index;
                            }
                            else
                            {
                                comboBox.Items.Add(option);
                            }
                        }
                        panel.Controls.Add(comboBox);
                        break;

                    case "CheckBox":
                        CheckBox checkBox = new CheckBox
                        {
                            Name = property.Name,
                            Width = 300,
                            Checked = property.Value.GetBoolean()
                        };
                        panel.Controls.Add(checkBox);
                        break;

                    case "TextBox":
                        SecureTextBox textBox = new SecureTextBox
                        {
                            Name = property.Name,
                            Width = 300,
                            Text = property.Value.GetString(),
                            InSecure = property.InSecure
                        };
                        panel.Controls.Add(textBox);
                        break;
                }
            }

            // Add Save button
            var saveButton = new Button
            {
                Text = "Save",
                Width = 100
            };
            saveButton.Click += SaveButton_Click;
            panel.Controls.Add(saveButton);

            this.Controls.Add(panel);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var updatedProperties = new Dictionary<string, object>();

            foreach (Control control in this.Controls[0].Controls) // Loop through the FlowLayoutPanel controls
            {
                if (control is ComboBox comboBox)
                {
                    updatedProperties[comboBox.Name] = comboBox.SelectedItem.ToString();
                    Logger.Trace(comboBox.SelectedItem.ToString());
                }
                else if (control is CheckBox checkBox)
                {
                    updatedProperties[checkBox.Name] = checkBox.Checked;
                    Logger.Trace(checkBox.Checked.ToString());
                }
                else if (control is TextBox textBox)
                {
                    updatedProperties[textBox.Name] = textBox.Text;
                    Logger.Trace(textBox.Text);
                }
            }

            // Now update the settings using your update method
            ModuleInstaller moduleInstaller = new ModuleInstaller();
            moduleInstaller.ChangeSettings(_moduleName, updatedProperties);
            MessageBox.Show("Settings updated successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Dispose();
        }
    }
}
