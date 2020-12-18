using OpenTPW.Files.FileFormats;
using System;
using System.Linq;
using System.Windows.Forms;

namespace TPWToolkit
{
    public partial class NewAssetPicker : Form
    {
        public NewAssetPicker()
        {
            InitializeComponent();
        }

        private void NewAssetPicker_Load(object sender, EventArgs e)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    // TODO: instantiating every time is BAD please fix
                    if (type.GetInterfaces().Contains(typeof(IAssetReader)))
                        mainListBox.Items.Add($"{type.GetProperty("AssetName").GetValue(Activator.CreateInstance(type))}");
                }
            }
            mainListBox.Sorted = true;
        }
    }
}
