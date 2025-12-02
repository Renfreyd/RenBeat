using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace MusicPlayer
{
    public partial class MusicPlayerForm : Form
    {
        private ListBox playlistBox;
        private ListBox songsBox;
        private Button btnAddSong;
        private Button btnRemoveSong;
        private Button btnPlay;
        private Button btnPause;
        private Button btnStop;
        private Button btnNext;
        private Button btnPrevious;
        private Button btnCreatePlaylist;
        private Button btnDeletePlaylist;
        private TrackBar volumeBar;
        private TrackBar progressBar;
        private Label lblCurrentSong;
        private Label lblVolume;
        private Label lblTime;
        private Timer timer;
        private CheckBox chkRepeat;
        private CheckBox chkShuffle;

        private IWavePlayer wavePlayer;
        private AudioFileReader audioFileReader;
        private List<Playlist> playlists;
        private Playlist currentPlaylist;
        private int currentSongIndex = -1;
        private bool isPlaying = false;
        private bool isDraggingProgress = false;

        public MusicPlayerForm()
        {
            InitializeComponents();
            playlists = new List<Playlist>();
            wavePlayer = new WaveOutEvent();
            wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
        }

        private void InitializeComponents()
        {
            this.Text = "Музыкальный плеер";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Панель плейлистов
            Label lblPlaylists = new Label
            {
                Text = "Плейлисты:",
                Location = new Point(20, 20),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblPlaylists);

            playlistBox = new ListBox
            {
                Location = new Point(20, 45),
                Size = new Size(200, 300),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            playlistBox.SelectedIndexChanged += PlaylistBox_SelectedIndexChanged;
            this.Controls.Add(playlistBox);

            btnCreatePlaylist = new Button
            {
                Text = "Создать плейлист",
                Location = new Point(20, 355),
                Size = new Size(95, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8)
            };
            btnCreatePlaylist.FlatAppearance.BorderSize = 0;
            btnCreatePlaylist.Click += BtnCreatePlaylist_Click;
            this.Controls.Add(btnCreatePlaylist);

            btnDeletePlaylist = new Button
            {
                Text = "Удалить",
                Location = new Point(125, 355),
                Size = new Size(95, 30),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8)
            };
            btnDeletePlaylist.FlatAppearance.BorderSize = 0;
            btnDeletePlaylist.Click += BtnDeletePlaylist_Click;
            this.Controls.Add(btnDeletePlaylist);

            // Панель песен
            Label lblSongs = new Label
            {
                Text = "Композиции:",
                Location = new Point(240, 20),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblSongs);

            songsBox = new ListBox
            {
                Location = new Point(240, 45),
                Size = new Size(620, 300),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            songsBox.DoubleClick += SongsBox_DoubleClick;
            this.Controls.Add(songsBox);

            btnAddSong = new Button
            {
                Text = "Добавить песню",
                Location = new Point(240, 355),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8)
            };
            btnAddSong.FlatAppearance.BorderSize = 0;
            btnAddSong.Click += BtnAddSong_Click;
            this.Controls.Add(btnAddSong);

            btnRemoveSong = new Button
            {
                Text = "Удалить песню",
                Location = new Point(380, 355),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8)
            };
            btnRemoveSong.FlatAppearance.BorderSize = 0;
            btnRemoveSong.Click += BtnRemoveSong_Click;
            this.Controls.Add(btnRemoveSong);

            // Панель управления
            Panel controlPanel = new Panel
            {
                Location = new Point(20, 410),
                Size = new Size(840, 130),
                BackColor = Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(controlPanel);

            lblCurrentSong = new Label
            {
                Text = "Нет воспроизведения",
                Location = new Point(10, 10),
                Size = new Size(820, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            controlPanel.Controls.Add(lblCurrentSong);

            progressBar = new TrackBar
            {
                Location = new Point(10, 40),
                Size = new Size(820, 30),
                Minimum = 0,
                Maximum = 100,
                TickStyle = TickStyle.None
            };
            progressBar.MouseDown += ProgressBar_MouseDown;
            progressBar.MouseUp += ProgressBar_MouseUp;
            progressBar.ValueChanged += ProgressBar_ValueChanged;
            controlPanel.Controls.Add(progressBar);

            lblTime = new Label
            {
                Text = "0:00 / 0:00",
                Location = new Point(370, 65),
                Size = new Size(100, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            controlPanel.Controls.Add(lblTime);

            // Кнопки управления
            btnPrevious = CreateControlButton("⏮", new Point(300, 85));
            btnPrevious.Click += BtnPrevious_Click;
            controlPanel.Controls.Add(btnPrevious);

            btnPlay = CreateControlButton("▶", new Point(360, 85));
            btnPlay.Click += BtnPlay_Click;
            controlPanel.Controls.Add(btnPlay);

            btnPause = CreateControlButton("⏸", new Point(420, 85));
            btnPause.Click += BtnPause_Click;
            controlPanel.Controls.Add(btnPause);

            btnStop = CreateControlButton("⏹", new Point(480, 85));
            btnStop.Click += BtnStop_Click;
            controlPanel.Controls.Add(btnStop);

            btnNext = CreateControlButton("⏭", new Point(540, 85));
            btnNext.Click += BtnNext_Click;
            controlPanel.Controls.Add(btnNext);

            // Громкость
            lblVolume = new Label
            {
                Text = "🔊 50%",
                Location = new Point(680, 90),
                Size = new Size(80, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8)
            };
            controlPanel.Controls.Add(lblVolume);

            volumeBar = new TrackBar
            {
                Location = new Point(760, 85),
                Size = new Size(70, 30),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickStyle = TickStyle.None
            };
            volumeBar.ValueChanged += VolumeBar_ValueChanged;
            controlPanel.Controls.Add(volumeBar);

            // Опции воспроизведения
            chkRepeat = new CheckBox
            {
                Text = "🔁 Повтор",
                Location = new Point(10, 90),
                Size = new Size(100, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            controlPanel.Controls.Add(chkRepeat);

            chkShuffle = new CheckBox
            {
                Text = "🔀 Случайно",
                Location = new Point(120, 90),
                Size = new Size(110, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            controlPanel.Controls.Add(chkShuffle);
        }

        private Button CreateControlButton(string text, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(50, 35),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BtnCreatePlaylist_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите название плейлиста:", "Новый плейлист", "Мой плейлист");

            if (!string.IsNullOrWhiteSpace(name))
            {
                Playlist playlist = new Playlist(name);
                playlists.Add(playlist);
                playlistBox.Items.Add(playlist.Name);
                MessageBox.Show($"Плейлист '{name}' создан!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDeletePlaylist_Click(object sender, EventArgs e)
        {
            if (playlistBox.SelectedIndex >= 0)
            {
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите удалить этот плейлист?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    playlists.RemoveAt(playlistBox.SelectedIndex);
                    playlistBox.Items.RemoveAt(playlistBox.SelectedIndex);
                    songsBox.Items.Clear();
                    currentPlaylist = null;
                }
            }
        }

        private void PlaylistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (playlistBox.SelectedIndex >= 0)
            {
                currentPlaylist = playlists[playlistBox.SelectedIndex];
                UpdateSongsList();
            }
        }

        private void UpdateSongsList()
        {
            songsBox.Items.Clear();
            if (currentPlaylist != null)
            {
                foreach (var song in currentPlaylist.Songs)
                {
                    songsBox.Items.Add(Path.GetFileName(song));
                }
            }
        }

        private void BtnAddSong_Click(object sender, EventArgs e)
        {
            if (currentPlaylist == null)
            {
                MessageBox.Show("Сначала выберите плейлист!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.wma;*.aac;*.flac;*.m4a|All Files|*.*",
                Multiselect = true,
                Title = "Выберите аудиофайлы"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    currentPlaylist.AddSong(file);
                }
                UpdateSongsList();
            }
        }

        private void BtnRemoveSong_Click(object sender, EventArgs e)
        {
            if (songsBox.SelectedIndex >= 0 && currentPlaylist != null)
            {
                currentPlaylist.Songs.RemoveAt(songsBox.SelectedIndex);
                UpdateSongsList();
            }
        }

        private void SongsBox_DoubleClick(object sender, EventArgs e)
        {
            if (songsBox.SelectedIndex >= 0)
            {
                currentSongIndex = songsBox.SelectedIndex;
                PlayCurrentSong();
            }
        }

        private void PlayCurrentSong()
        {
            if (currentPlaylist != null && currentSongIndex >= 0 &&
                currentSongIndex < currentPlaylist.Songs.Count)
            {
                string song = currentPlaylist.Songs[currentSongIndex];
                try
                {
                    // Остановка предыдущего воспроизведения
                    wavePlayer.Stop();
                    audioFileReader?.Dispose();

                    // Загрузка нового файла
                    audioFileReader = new AudioFileReader(song);
                    wavePlayer.Init(audioFileReader);

                    // Установка громкости
                    audioFileReader.Volume = volumeBar.Value / 100f;

                    // Воспроизведение
                    wavePlayer.Play();
                    isPlaying = true;

                    lblCurrentSong.Text = $"▶ {Path.GetFileNameWithoutExtension(song)}";
                    timer.Start();

                    // Подсветка текущей песни
                    songsBox.SelectedIndex = currentSongIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка воспроизведения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (audioFileReader != null && !isPlaying)
            {
                wavePlayer.Play();
                isPlaying = true;
                timer.Start();
            }
            else if (currentSongIndex >= 0)
            {
                PlayCurrentSong();
            }
            else if (currentPlaylist != null && currentPlaylist.Songs.Count > 0)
            {
                currentSongIndex = 0;
                PlayCurrentSong();
            }
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                wavePlayer.Pause();
                isPlaying = false;
                timer.Stop();
                lblCurrentSong.Text = lblCurrentSong.Text.Replace("▶", "⏸");
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            wavePlayer.Stop();
            isPlaying = false;
            timer.Stop();
            lblCurrentSong.Text = "Остановлено";
            progressBar.Value = 0;
            lblTime.Text = "0:00 / 0:00";

            if (audioFileReader != null)
            {
                audioFileReader.Position = 0;
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentPlaylist != null)
            {
                if (chkShuffle.Checked)
                {
                    Random rand = new Random();
                    currentSongIndex = rand.Next(0, currentPlaylist.Songs.Count);
                }
                else if (currentSongIndex < currentPlaylist.Songs.Count - 1)
                {
                    currentSongIndex++;
                }
                else if (chkRepeat.Checked)
                {
                    currentSongIndex = 0;
                }
                PlayCurrentSong();
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (audioFileReader != null && audioFileReader.CurrentTime.TotalSeconds > 3)
            {
                // Если прошло больше 3 секунд, начать песню сначала
                audioFileReader.Position = 0;
            }
            else if (currentSongIndex > 0)
            {
                currentSongIndex--;
                PlayCurrentSong();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFileReader != null && !isDraggingProgress)
            {
                // Обновление прогресса
                if (audioFileReader.TotalTime.TotalSeconds > 0)
                {
                    int progress = (int)((audioFileReader.CurrentTime.TotalSeconds /
                        audioFileReader.TotalTime.TotalSeconds) * 100);
                    progressBar.Value = Math.Min(progress, 100);

                    // Обновление времени
                    lblTime.Text = $"{FormatTime(audioFileReader.CurrentTime)} / {FormatTime(audioFileReader.TotalTime)}";
                }
            }
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Автоматическое переключение на следующую песню
            if (isPlaying && currentPlaylist != null)
            {
                if (chkRepeat.Checked || currentSongIndex < currentPlaylist.Songs.Count - 1)
                {
                    BtnNext_Click(null, null);
                }
                else
                {
                    isPlaying = false;
                    lblCurrentSong.Text = "Плейлист завершен";
                }
            }
        }

        private void VolumeBar_ValueChanged(object sender, EventArgs e)
        {
            if (audioFileReader != null)
            {
                audioFileReader.Volume = volumeBar.Value / 100f;
            }
            lblVolume.Text = $"🔊 {volumeBar.Value}%";
        }

        private void ProgressBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggingProgress = true;
        }

        private void ProgressBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggingProgress = false;
            if (audioFileReader != null)
            {
                double newPosition = (progressBar.Value / 100.0) * audioFileReader.TotalTime.TotalSeconds;
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(newPosition);
            }
        }

        private void ProgressBar_ValueChanged(object sender, EventArgs e)
        {
            if (isDraggingProgress && audioFileReader != null)
            {
                double newPosition = (progressBar.Value / 100.0) * audioFileReader.TotalTime.TotalSeconds;
                lblTime.Text = $"{FormatTime(TimeSpan.FromSeconds(newPosition))} / {FormatTime(audioFileReader.TotalTime)}";
            }
        }

        private string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            wavePlayer?.Stop();
            audioFileReader?.Dispose();
            wavePlayer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}