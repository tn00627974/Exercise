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
            Creature creature = new Villager();
            MessageBox.Show(creature.Say());

            Creature player = new Player();
            MessageBox.Show(player.Say());

            Monster monster = new Monster();
            MessageBox.Show(monster.Say());
        }
    }
}
