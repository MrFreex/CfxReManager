using CfxRe_Manager_by_Mrfreex.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CfxRe_Manager_by_Mrfreex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (!Directory.Exists(@"C:\\ProgramData\Fx_manager"))
            {
                Directory.CreateDirectory(@"C:\\ProgramData\Fx_manager");
            }
            comboEnStart.Items.Add("Start");
            comboEnStart.Items.Add("Ensure");
            XDocument master;
            
            if (File.Exists(fx_general.path + @"\" + "config.xml"))
            {
                master = XDocument.Load(fx_general.path + @"\config.xml");
                string Profile = master.Root.Element("SProfile").Value.Replace("<SProfile>", "").Replace("</SProfile>", "");
                string[] Profiles = master.Root.Element("Profiles").Value.Replace("<Profiles>", "").Replace("</Profiles>", "").Split(':');
                
                for (int i = 0; i < Profiles.Length; i++)
                {
                    if (Profiles[i] != "")
                    {
                        comboBox1.Items.Add(Profiles[i]);
                    }
                    
                }
                comboBox1.SelectedItem = Profile;
                loadProfile(Profile);
            }
            else
            {
                master = new XDocument(
                            new XDeclaration("1.0", "utf8", "yes"),
                            new XElement("General",
                             new XElement("SProfile", ""),
                             new XElement("Profiles", "")
                            )

                    );
                master.Save(fx_general.path + @"\" + @"config.xml");
                //saveProfile("Default");
            }
            
            startUp();

        }

        public static class fx_general
        {
            public static string path = @"C:\\ProgramData\Fx_manager";
            public static string bg = @"CfxRe_Manager_by_Mrfreex.Properties.Resources.bg1";
            public static int game = 0;
        }

        private void launchServer()
        {
            string FXServerexe = textBox1.Text;
            string servercfg = textBox2.Text + @"\server.cfg";
            string data = textBox2.Text;

            if (File.Exists(fx_general.path + @"\bat.bat")) {
                File.Delete(fx_general.path + @"\bat.bat");
            }

            StreamWriter bat = File.CreateText(fx_general.path + @"\bat.bat");

            bat.WriteLine(string.Format("cd /d \"{0}\"", data));
            if (comboBox3.SelectedIndex == 1)
            {
                bat.WriteLine(string.Format("\"{0}\" +exec \"{1}\" +set gamename rdr3", FXServerexe, servercfg));
            } else
            {
                bat.WriteLine(string.Format("\"{0}\" +exec \"{1}\"", FXServerexe, servercfg));
            }
            

            bat.Dispose();

            System.Diagnostics.Process.Start(fx_general.path + @"\bat.bat");
        }

        private void startUp()
        {
            checkedListBox1.Items.Clear();
            if (File.Exists(textBox2.Text + @"\server.cfg"))
            {
                String[] file = File.ReadLines(textBox2.Text + @"\server.cfg").ToArray();
                List<string> list = new List<string>();
                for (int i = 0; i < file.Length; i++)
                {
                    string res = file[i];
                    if (res.Contains("Start ") || res.Contains("Ensure "))
                    {
                        list.Add(res.Replace("Start ", "").Replace("Ensure ", ""));
                        checkedListBox1.Items.Add(res.Replace("Start ", "").Replace("Ensure ", ""));
                        
                    }
                }
                file = list.ToArray();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (file[i].Contains(checkedListBox1.Items[i].ToString()))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }

            if (Directory.Exists(textBox2.Text) && File.Exists(textBox2.Text + @"\server.cfg"))
            {
                String[] resources = Directory.GetDirectories(textBox2.Text + @"\resources", "*", SearchOption.AllDirectories);
                List<string> newResources = new List<string>();
               
                for (int i = 0; i < resources.Length; i++)
                {
                    if (File.Exists(resources[i] + @"\fxmanifest.lua") || File.Exists(resources[i] + @"\__resource.lua"))
                    {
                        newResources.Add(resources[i]);

                        //MessageBox.Show(resources[i]);
                    }
                }
                
                resources = newResources.ToArray();
                for (int i = 0; i < resources.Length; i++)
                {
                     string text = resources[i].Split(Convert.ToChar(@"\"))[resources[i].Split(Convert.ToChar(@"\")).Length - 1];
                    if (!checkedListBox1.Items.Contains(text))
                    {
                        checkedListBox1.Items.Add(text);
                    }   
                }
            }

            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://keymaster.fivem.net/");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            XDocument master;
            master = XDocument.Load(fx_general.path + @"\" + @"config.xml");
            master.Root.Element("SProfile").Value = comboBox1.Text;
            master.Save(fx_general.path + @"\" + @"config.xml");
        }

        private void saveProfile (string Profile)
        {
            if (!Directory.Exists(@"C:\\ProgramData\Fx_manager\profiles")) 
            {
                Directory.CreateDirectory(@"C:\\ProgramData\Fx_manager\profiles");
            }

            if (Profile == "")
            {
                Profile = comboBox1.SelectedItem.ToString();
            }

            if (!comboBox1.Items.Contains(Profile))
            {
                comboBox1.Items.Add(Profile);
            }

            XDocument master;
            if (File.Exists(fx_general.path + @"\" + "config.xml"))
            {
                master = XDocument.Load(fx_general.path + @"\" + @"config.xml");
                master.Root.Element("SProfile").Value = Profile;
                master.Root.Element("Profiles").Value = listProfiles();
                master.Save(fx_general.path + @"\" + @"config.xml");
            }
            else
            {
                master = new XDocument(
                            new XDeclaration("1.0", "utf8", "yes"),
                            new XElement("General",
                             new XElement("SProfile", Profile),
                             new XElement("Profiles", listProfiles())
                            )

                    );
                master.Save(fx_general.path + @"\" + @"config.xml");
            }
            if (File.Exists(fx_general.path + @"\profiles\" + Profile))
            {
                File.Delete(fx_general.path + @"\profiles\" + Profile);
            }
            string addToConfig = "";
            for (int i = 0; i < textBox13.Lines.Length; i++)
            {
                if (textBox13.Lines[i] == "")
                {

                } else
                {
                    addToConfig += ":" + textBox13.Lines[i];
                }
            }
            XDocument profileConfig = new XDocument(
              new XDeclaration("1.0", "utf8", "yes"),
              new XComment("XML Config auto generated by Password Generator"),
              new XComment("Authors: MrFreex"),
              new XElement(Profile,
                  new XElement("FXServer.exe", textBox1.Text),
                  new XElement("Data", textBox2.Text),
                  new XElement("Pnumber", textBox3.Text),
                  new XElement("Port", textBox4.Text),
                  new XElement("license",textBox5.Text),
                  new XElement ("resources",XMLResources()),
                  new XElement ( "startType", comboEnStart.Text),
                  new XElement ( "style", comboBox2.SelectedIndex),
                  new XElement ("dbuser", textBox7.Text),
                  new XElement ("dbip", textBox6.Text),
                  new XElement ("dbpass", textBox11.Text),
                  new XElement ("hostname", textBox9.Text),
                  new XElement ("locale", textBox10.Text),
                  new XElement ("tags", textBox14.Text),
                  new XElement ("game", comboBox3.SelectedIndex),
                  new XElement ("addAfterCfg", addToConfig),
                  new XElement ("dbName", textBox8.Text),
                  new XElement ("useString", checkBox1.Checked.ToString())
              )
           );

            profileConfig.Save(fx_general.path + @"\profiles\" + Profile + @".xml");
        }

        private void loadProfile(string Profile)
        {
            XDocument master;
            if (File.Exists(fx_general.path + @"\" + "config.xml"))
            {
                master = XDocument.Load(fx_general.path + @"\config.xml");
            } else
            {
                master = new XDocument(
                            new XDeclaration("1.0", "utf8", "yes"),
                            new XElement ("General",
                             new XElement("SProfile","Default"),
                             new XElement("Profiles", " ")
                            )

                    );
                master.Save(fx_general.path + @"\" + @"config.xml");
                saveProfile("Default");
            }
            if (master.Root.Element("Profiles").Value.Replace("<Profiles>", "").Replace("</Profiles>", "") == "")
            {
                return;
            }
            if (Profile == "")
            {
                Profile = comboBox1.Text;
            }
            XDocument profileConfig = XDocument.Load(fx_general.path + @"\profiles\" + Profile + @".xml");
            textBox1.Text = xmlElement("FXServer.exe", Profile);
            textBox2.Text = xmlElement("Data", Profile);
            textBox3.Text = xmlElement("Pnumber", Profile);
            textBox4.Text = xmlElement("Port", Profile);
            textBox5.Text = xmlElement("license", Profile);
            comboEnStart.SelectedItem = xmlElement("startType", Profile);
            comboBox2.SelectedIndex = int.Parse(xmlElement("style", Profile));
            textBox6.Text = xmlElement("dbip", Profile);
            textBox7.Text = xmlElement("dbuser", Profile);
            textBox11.Text = xmlElement("dbpass", Profile);
            textBox9.Text = xmlElement("hostname", Profile);
            textBox10.Text = xmlElement("locale", Profile);
            textBox14.Text = xmlElement("tags", Profile);
            textBox8.Text = xmlElement("dbName", Profile);
            checkBox1.Checked = bool.Parse(xmlElement("useString", Profile));
            string[] vs = xmlElement("addAfterCfg", Profile).Split(':');
            textBox13.Clear();
            for (int i = 0; i < vs.Length; i++)
            {
                if (vs[i] != ":")
                {
                    if (vs[i] != "")
                    {
                        textBox13.AppendText(vs[i] + Environment.NewLine);
                    }
                }
            }
            comboBox3.SelectedIndex = int.Parse(xmlElement("game", Profile));
        }

        private string xmlElement(string name, string Profile)
        {
            XDocument profileConfig = XDocument.Load(fx_general.path + @"\profiles\" + Profile + @".xml");
            string output = profileConfig.Root.Element(name).Value.Replace(@"<" + name + @">", "").Replace(@"<" + name + @">", "");
            return output;
        }

        private void updateProfile (string Profile)
        {
            XDocument config = XDocument.Load(fx_general.path + @"\profiles\" + Profile + @".xml");
            config.Root.Element("FXServer.exe").Value = textBox1.Text;
            config.Root.Element("Data").Value = textBox2.Text;
            config.Root.Element("Pnumber").Value = textBox3.Text;
            config.Root.Element("Port").Value = textBox4.Text;
            config.Root.Element("license").Value = textBox5.Text;
            config.Root.Element("resources").Value = XMLResources();
            config.Root.Element("startType").Value = comboEnStart.Text;
            if (comboBox2.SelectedItem.ToString() == "")
            {
                comboBox2.SelectedIndex = 1;
            }
            config.Root.Element("style").Value = comboBox2.SelectedIndex.ToString();
            config.Root.Element("dbip").Value = textBox6.Text;
            config.Root.Element("dbuser").Value = textBox7.Text;
            config.Root.Element("dbpass").Value = textBox11.Text;
            config.Root.Element("hostname").Value = textBox9.Text;
            config.Root.Element("locale").Value = textBox10.Text;
            config.Root.Element("tags").Value = textBox14.Text;
            config.Root.Element("game").Value = string.Format("{0}", comboBox3.SelectedIndex);
            config.Root.Element("useString").Value = checkBox1.Checked.ToString();
            config.Root.Element("dbName").Value = textBox8.Text;
            config.Save(fx_general.path + @"\profiles\" + Profile + @".xml");
        }

        private string XMLResources ()
        {
            string res = "";
            for (int i = 0; i < (checkedListBox1.Items.Count); i++)
            {
                res += (@":" + checkedListBox1.Items[i]);
            }
            
            
            return res;
        }

        private void button3_Click(object sender, EventArgs e) // Resources Down
        {
            if (checkedListBox1.SelectedIndex == checkedListBox1.Items.Count - 1)
            {
                return;
            }
            object Item = checkedListBox1.Items[checkedListBox1.SelectedIndex];
            checkedListBox1.Items[checkedListBox1.SelectedIndex] = checkedListBox1.Items[checkedListBox1.SelectedIndex + 1];
            checkedListBox1.Items[checkedListBox1.SelectedIndex + 1] = Item;
            checkedListBox1.SelectedIndex = checkedListBox1.SelectedIndex + 1;
        }

        private void button2_Click(object sender, EventArgs e) // Resources UP
        {
            if (checkedListBox1.SelectedIndex == 0)
            {
                return;
            }
            object Item = checkedListBox1.Items[checkedListBox1.SelectedIndex];
            checkedListBox1.Items[checkedListBox1.SelectedIndex] = checkedListBox1.Items[checkedListBox1.SelectedIndex - 1];
            checkedListBox1.Items[checkedListBox1.SelectedIndex - 1] = Item;
            checkedListBox1.SelectedIndex = checkedListBox1.SelectedIndex - 1;
        }

        private void saveProfBtn_Click(object sender, EventArgs e)
        {
            
            saveProfile(comboBox1.Text);
            generateConfig();
            string file = File.ReadAllText(fx_general.path + @"\temp.txt");
            if (textBox2.Text == "")
            {
                return;
            }
            StreamWriter config = File.CreateText(textBox2.Text + @"\server.cfg");
            config.Write(file);
            config.Dispose();
        }

        private void loadProfBtn_Click(object sender, EventArgs e)
        {
            loadProfile("");
        }

        private string listProfiles ()
        {
            string res = "";
            for (int i = 0; i < (comboBox1.Items.Count); i++)
            {
                res += (@":" + comboBox1.Items[i]);
            }
            return res;
        }

        private void browseFXSVBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath + @"\FXServer.exe";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            textBox2.Text = fbd.SelectedPath;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool ddd;
            if (comboBox2.SelectedIndex == 0)
            {
                ddd = true;
            } else
            {
                ddd = false;
            }
            changeBg(ddd);
        }

        private void changeBg(bool red)
        {
            System.Drawing.Bitmap bitmap = Properties.Resources.bg1;

            if (red)
            {
                bitmap = Properties.Resources.bg2;
            }
            
            for (int i = 0; i < (tabControl1.TabCount); i++)
            {
                tabControl1.TabPages[i].BackgroundImage = bitmap;
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MrFreex");
        }

        private void launchBtn_Click(object sender, EventArgs e)
        {
            generateConfig();
            string file = File.ReadAllText(fx_general.path + @"\temp.txt");
            StreamWriter config = File.CreateText(textBox2.Text + @"\server.cfg");
            config.Write(file);
            config.Dispose();
            launchServer();
        }

        private void generateConfig()
        {
            if (File.Exists(fx_general.path + @"\temp.txt"))
            {
                File.Delete(fx_general.path + @"\temp.txt");
            }
            StreamWriter file = File.CreateText(fx_general.path + @"\temp.txt");
            if (checkBox1.Checked)
            {
                string password = "";
                if (textBox11.Text != "")
                {
                    password = ":";
                    password += textBox11.Text;
                }
                file.WriteLine("set mysql_connection_string \"mysql://" + textBox7.Text + password + "@" + textBox6.Text + "/" + textBox8.Text + "\"");
            }
            file.WriteLine("endpoint_add_tcp \"0.0.0.0:" + textBox4.Text + "\"");
            file.WriteLine("endpoint_add_udp \"0.0.0.0:" + textBox4.Text + "\"");

            for (int i = 0; i < (checkedListBox1.CheckedItems.Count); i++)
            {
                file.WriteLine(comboEnStart.Text + " " + checkedListBox1.CheckedItems[i].ToString());
            }
            if (textBox14.Text != "")
            {
                file.WriteLine("sets tags " + "\"" + textBox14.Text + "\"");
            }

            file.WriteLine("sv_hostname " + "\"" + textBox9.Text + "\"");
            file.WriteLine("sv_maxclients " + textBox3.Text);
            file.WriteLine("sv_licensekey " + textBox5.Text);
            file.WriteLine("sets locale " + textBox10.Text);
            for (int i = 0; i < textBox13.Lines.Length; i++)
            {
                file.WriteLine(textBox13.Lines[i]);
            }
            file.Dispose();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            startUp();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            generateConfig();
            string file = File.ReadAllText(fx_general.path + @"\temp.txt");
            System.Windows.Forms.Clipboard.SetText(file);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            fx_general.game = comboBox3.SelectedIndex;
        }
    }
}
