namespace UME
{
    public partial class ShowForm : Form
    {
        public ShowForm()
        {
            InitializeComponent();
        }
        public void setPicture(Bitmap img)
        {
            pictureBox1.Image = img;
            Invalidate();
        }
        private void ShowForm_Load(object sender, EventArgs e)
        {

        }
    }
}
