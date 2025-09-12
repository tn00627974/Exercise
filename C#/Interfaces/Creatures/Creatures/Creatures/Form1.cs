using System.Windows.Forms;

namespace Creatures
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg;

            // 建立生物
            Creature villager = new Villager();
            Creature monster = new Monster();
            Creature player = new Player();

            // 開場白
            ListBox_Log(monster.Say());
            ListBox_Log(player.Say());
            ListBox_Log(villager.Say());

            // 戰鬥
            for (int i = 0; i < 4; i++)
            {
                msg = monster.Attack(player);
                ListBox_Log(msg);
            }
            
            // 對話
            ITalkable playerTalkTo = new Player();
            msg = playerTalkTo.TalkTo(villager);
            ListBox_Log(msg);

        }
        private void ListBox_Log(string message)
        {
            listBox1.Items.Add(message);
            listBox1.TopIndex = listBox1.Items.Count - 1; // 保持捲到最底
        }
    }
}
