using System.Data.SqlClient;

namespace WFAC220221
{
    public partial class FrmMain : Form
    {
        private string? _SelectedId = "-1";
        private string? _NextFreeId = "-1";

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Reload()
        {
            dgvMain.Rows.Clear();

            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();

            var reader = new SqlCommand("SELECT * FROM tanverseny;", connection)
                .ExecuteReader();

            while (reader.Read()) dgvMain.Rows.Add(
                reader[0], reader[1], reader[2], reader[3],
                reader.GetDateTime(4).ToString("yyyy. MM. dd."));
        }
        private void GetCompetitionTypes()
        {
            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();
            var reader = new SqlCommand(
                "SELECT DISTINCT tipus FROM tanverseny;", connection)
                .ExecuteReader();
            while (reader.Read())
                cbVersenyTipusok.Items.Add(reader[0]);
        }
        private void GetEventTypes()
        {
            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();
            var reader = new SqlCommand(
                "SELECT DISTINCT esemeny FROM tanverseny;", connection)
                .ExecuteReader();
            while (reader.Read())
                cbEsemenyTipusok.Items.Add(reader[0]);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            GetCompetitionTypes();
            GetEventTypes();
            Reload();
        }

        private void SetNextFreeId()
        {
            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();
            var reader = new SqlCommand(
                "SELECT MAX(azonosito) + 1 FROM tanverseny;", connection)
                .ExecuteReader();
            reader.Read();
            _NextFreeId = $"{reader[0]}";
        }

        private void DgvMain_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _SelectedId = dgvMain[0, e.RowIndex].Value.ToString();
            tbVersenyNev.Text = dgvMain[1, e.RowIndex].Value.ToString();
            cbVersenyTipusok.Text = dgvMain[2, e.RowIndex].Value.ToString();
            cbEsemenyTipusok.Text = dgvMain[3, e.RowIndex].Value.ToString();
            dtpEsemenyDatum.Value = Convert.ToDateTime(dgvMain[4, e.RowIndex].Value);
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            Reload();
            ClearFields();
        }

        private void ClearFields()
        {
            tbVersenyNev.Text = string.Empty;
            cbVersenyTipusok.SelectedIndex = -1;
            cbVersenyTipusok.Text = string.Empty;
            cbEsemenyTipusok.SelectedIndex = -1;
            cbEsemenyTipusok.Text = string.Empty;
            dtpEsemenyDatum.Value = DateTime.Now;
            _SelectedId = "-1";
            _NextFreeId = "-1";
        }

        private bool IsAllFieldOk()
        {
            string errorMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(tbVersenyNev.Text))
                errorMsg += $"A verseny neve nem lehet üres!\n";
            if (string.IsNullOrWhiteSpace(cbVersenyTipusok.Text))
                errorMsg += $"A válaszd ki a verseny típusát!\n";
            if (string.IsNullOrWhiteSpace(cbEsemenyTipusok.Text))
                errorMsg += $"A válaszd ki az esemény típusát!\n";

            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(
                    caption: "HIBA!",
                    text: $"{errorMsg}",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private void Success()
        {
            MessageBox.Show(
                caption: "SIKER",
                text: $"A módosítás sikeresen megtörtént!\n" +
                      $"Frissítsd a nézetet az új adatok betöltéséhez!",
                icon: MessageBoxIcon.Information,
                buttons: MessageBoxButtons.OK);
            ClearFields();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_SelectedId == "-1")
            {
                MessageBox.Show(
                    caption: "HIBA!",
                    text: $"Elõször jelölj ki egy sort a táblázatban!",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK);
                return;
            }

            var res = MessageBox.Show(
                caption: "MÓDOSÍTÁS",
                text: $"Biztosan módosítani akarod az adatbázis {_SelectedId}. azonosítójú bejegyzését?",
                icon: MessageBoxIcon.Warning,
                buttons: MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            if (!IsAllFieldOk()) return;

            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();

            var adapter = new SqlDataAdapter()
            {
                UpdateCommand = new SqlCommand(
                    "UPDATE tanverseny SET " +
                    $"versenynev = '{tbVersenyNev.Text}', " +
                    $"tipus = '{cbVersenyTipusok.Text}', " +
                    $"esemeny = '{cbEsemenyTipusok.Text}', " +
                    $"datum = '{dtpEsemenyDatum.Value.ToString("yyyy-MM-dd")}' " +
                    $"WHERE azonosito = {_SelectedId};", connection),
            };

            adapter.UpdateCommand.ExecuteNonQuery();
            Success();
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            SetNextFreeId();

            var res = MessageBox.Show(
                caption: "HOZZÁADÁS",
                text: $"Biztosan szeretnéd hozzáadni az adatbázishoz a {_NextFreeId}. azonosítójú bejegyzését?",
                icon: MessageBoxIcon.Warning,
                buttons: MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            if (!IsAllFieldOk()) return;

            using SqlConnection connection = new(Program.ConnectionString);
            connection.Open();

            var adapter = new SqlDataAdapter()
            {
                InsertCommand = new SqlCommand(
                    "INSERT INTO tanverseny VALUES (" +
                    $"{_NextFreeId}, " +
                    $"'{tbVersenyNev.Text}', " +
                    $"'{cbVersenyTipusok.Text}', " +
                    $"'{cbEsemenyTipusok.Text}', " +
                    $"'{dtpEsemenyDatum.Value.ToString("yyyy-MM-dd")}');",
                    connection),
            };

            adapter.InsertCommand.ExecuteNonQuery();
            Success();
        }
    }
}