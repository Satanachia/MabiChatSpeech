﻿namespace MabiChatSpeech
{
    partial class Main
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            SLB_Mode = new System.Windows.Forms.ToolStripStatusLabel();
            SLB_SaveMode = new System.Windows.Forms.ToolStripStatusLabel();
            SLB_Client = new System.Windows.Forms.ToolStripStatusLabel();
            SLB_Ip = new System.Windows.Forms.ToolStripStatusLabel();
            BTN_SendTask = new System.Windows.Forms.ToolStripDropDownButton();
            Btn_Redirect = new System.Windows.Forms.ToolStripStatusLabel();
            Txt_Chat = new System.Windows.Forms.TextBox();
            Tim_Status = new System.Windows.Forms.Timer(components);
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            Btn_ChatList = new System.Windows.Forms.ToolStripButton();
            Btn_Clear = new System.Windows.Forms.ToolStripButton();
            Btn_DumpView = new System.Windows.Forms.ToolStripButton();
            Btn_Setup = new System.Windows.Forms.ToolStripButton();
            Btn_List = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            Cmb_Whitelist = new System.Windows.Forms.ToolStripComboBox();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            Cmb_User = new System.Windows.Forms.ToolStripComboBox();
            toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            Cmb_Npc = new System.Windows.Forms.ToolStripComboBox();
            statusStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { SLB_Mode, SLB_SaveMode, SLB_Client, SLB_Ip, BTN_SendTask, Btn_Redirect });
            statusStrip1.Location = new System.Drawing.Point(0, 231);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            statusStrip1.ShowItemToolTips = true;
            statusStrip1.Size = new System.Drawing.Size(846, 24);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // SLB_Mode
            // 
            SLB_Mode.Name = "SLB_Mode";
            SLB_Mode.Size = new System.Drawing.Size(38, 19);
            SLB_Mode.Text = "Chat";
            SLB_Mode.ToolTipText = "Mode";
            // 
            // SLB_SaveMode
            // 
            SLB_SaveMode.AutoSize = false;
            SLB_SaveMode.Name = "SLB_SaveMode";
            SLB_SaveMode.Size = new System.Drawing.Size(100, 19);
            SLB_SaveMode.Text = "しない";
            SLB_SaveMode.ToolTipText = "チャットログの保存方法";
            // 
            // SLB_Client
            // 
            SLB_Client.AutoSize = false;
            SLB_Client.Name = "SLB_Client";
            SLB_Client.Size = new System.Drawing.Size(70, 19);
            SLB_Client.Text = "OFF";
            SLB_Client.ToolTipText = "接続状態";
            // 
            // SLB_Ip
            // 
            SLB_Ip.AutoSize = false;
            SLB_Ip.Image = Properties.Resources.icn_ruairi;
            SLB_Ip.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            SLB_Ip.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            SLB_Ip.Name = "SLB_Ip";
            SLB_Ip.Size = new System.Drawing.Size(120, 19);
            SLB_Ip.Text = "000";
            SLB_Ip.ToolTipText = "接続先";
            // 
            // BTN_SendTask
            // 
            BTN_SendTask.AutoSize = false;
            BTN_SendTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            BTN_SendTask.Image = (System.Drawing.Image)resources.GetObject("BTN_SendTask.Image");
            BTN_SendTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            BTN_SendTask.Name = "BTN_SendTask";
            BTN_SendTask.Size = new System.Drawing.Size(200, 22);
            BTN_SendTask.Text = "リダイレクト先";
            BTN_SendTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BTN_SendTask.ToolTipText = "リダイレクト先タスク";
            BTN_SendTask.DropDownOpening += BTN_SendTask_DropDownOpening;
            BTN_SendTask.DropDownItemClicked += BTN_SendTask_DropDownItemClicked;
            // 
            // Btn_Redirect
            // 
            Btn_Redirect.Name = "Btn_Redirect";
            Btn_Redirect.Size = new System.Drawing.Size(34, 19);
            Btn_Redirect.Text = "OFF";
            Btn_Redirect.ToolTipText = "リダイレクトのONOFF";
            Btn_Redirect.Click += Btn_Redirect_Click;
            // 
            // Txt_Chat
            // 
            Txt_Chat.BackColor = System.Drawing.Color.Black;
            Txt_Chat.Dock = System.Windows.Forms.DockStyle.Fill;
            Txt_Chat.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            Txt_Chat.ForeColor = System.Drawing.Color.Lime;
            Txt_Chat.Location = new System.Drawing.Point(0, 27);
            Txt_Chat.Margin = new System.Windows.Forms.Padding(5);
            Txt_Chat.Multiline = true;
            Txt_Chat.Name = "Txt_Chat";
            Txt_Chat.ReadOnly = true;
            Txt_Chat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            Txt_Chat.Size = new System.Drawing.Size(846, 204);
            Txt_Chat.TabIndex = 0;
            Txt_Chat.WordWrap = false;
            // 
            // Tim_Status
            // 
            Tim_Status.Interval = 1000;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Btn_ChatList, Btn_Clear, Btn_DumpView, Btn_Setup, Btn_List, toolStripSeparator1, toolStripLabel4, Cmb_Whitelist, toolStripLabel1, Cmb_User, toolStripLabel3, Cmb_Npc });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(846, 27);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // Btn_ChatList
            // 
            Btn_ChatList.Image = Properties.Resources.NewTeamProject;
            Btn_ChatList.ImageTransparentColor = System.Drawing.Color.Magenta;
            Btn_ChatList.Name = "Btn_ChatList";
            Btn_ChatList.Size = new System.Drawing.Size(43, 24);
            Btn_ChatList.Text = "ID";
            Btn_ChatList.ToolTipText = "ID";
            Btn_ChatList.Visible = false;
            Btn_ChatList.Click += Btn_Add_Click;
            // 
            // Btn_Clear
            // 
            Btn_Clear.Image = Properties.Resources.ClearWindowContent;
            Btn_Clear.ImageTransparentColor = System.Drawing.Color.Magenta;
            Btn_Clear.Name = "Btn_Clear";
            Btn_Clear.Size = new System.Drawing.Size(61, 24);
            Btn_Clear.Text = "クリア";
            Btn_Clear.ToolTipText = "テキストクリア";
            Btn_Clear.Click += Btn_Clear_Click;
            // 
            // Btn_DumpView
            // 
            Btn_DumpView.Image = Properties.Resources.imageres;
            Btn_DumpView.ImageTransparentColor = System.Drawing.Color.Magenta;
            Btn_DumpView.Name = "Btn_DumpView";
            Btn_DumpView.Size = new System.Drawing.Size(67, 24);
            Btn_DumpView.Text = "Dump";
            Btn_DumpView.Visible = false;
            Btn_DumpView.Click += Btn_DumpView_Click;
            // 
            // Btn_Setup
            // 
            Btn_Setup.Image = Properties.Resources.SettingsPanel;
            Btn_Setup.ImageTransparentColor = System.Drawing.Color.Magenta;
            Btn_Setup.Name = "Btn_Setup";
            Btn_Setup.Size = new System.Drawing.Size(57, 24);
            Btn_Setup.Text = "設定";
            Btn_Setup.ToolTipText = "各種設定";
            Btn_Setup.Click += Btn_Setup_Click;
            // 
            // Btn_List
            // 
            Btn_List.Image = Properties.Resources.TeamProject;
            Btn_List.ImageTransparentColor = System.Drawing.Color.Magenta;
            Btn_List.Name = "Btn_List";
            Btn_List.Size = new System.Drawing.Size(60, 24);
            Btn_List.Text = "リスト";
            Btn_List.ToolTipText = "キャラクターリスト";
            Btn_List.Click += Btn_List_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel4
            // 
            toolStripLabel4.Name = "toolStripLabel4";
            toolStripLabel4.Size = new System.Drawing.Size(68, 24);
            toolStripLabel4.Text = "キャラ指定";
            // 
            // Cmb_Whitelist
            // 
            Cmb_Whitelist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            Cmb_Whitelist.DropDownWidth = 60;
            Cmb_Whitelist.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            Cmb_Whitelist.Items.AddRange(new object[] { "OFF", "ChatOnly", "Voice" });
            Cmb_Whitelist.Name = "Cmb_Whitelist";
            Cmb_Whitelist.Size = new System.Drawing.Size(99, 27);
            Cmb_Whitelist.ToolTipText = "リストに登録したキャラのみ";
            Cmb_Whitelist.SelectedIndexChanged += Cmb_Whitelist_SelectedIndexChanged;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(37, 24);
            toolStripLabel1.Text = "User";
            // 
            // Cmb_User
            // 
            Cmb_User.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            Cmb_User.DropDownWidth = 60;
            Cmb_User.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            Cmb_User.Items.AddRange(new object[] { "OFF", "ChatOnly", "Voice 1", "Voice 2" });
            Cmb_User.Name = "Cmb_User";
            Cmb_User.Size = new System.Drawing.Size(99, 27);
            Cmb_User.ToolTipText = "一般ユーザーすべて";
            Cmb_User.SelectedIndexChanged += Cmb_User_SelectedIndexChanged;
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new System.Drawing.Size(33, 24);
            toolStripLabel3.Text = "Npc";
            // 
            // Cmb_Npc
            // 
            Cmb_Npc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            Cmb_Npc.DropDownWidth = 60;
            Cmb_Npc.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            Cmb_Npc.Items.AddRange(new object[] { "OFF", "ChatOnly", "Voice 1", "Voice 2" });
            Cmb_Npc.Name = "Cmb_Npc";
            Cmb_Npc.Size = new System.Drawing.Size(99, 27);
            Cmb_Npc.ToolTipText = "その他のキャラ";
            Cmb_Npc.SelectedIndexChanged += Cmb_Npc_SelectedIndexChanged;
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(846, 255);
            Controls.Add(Txt_Chat);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(5);
            MinimumSize = new System.Drawing.Size(861, 291);
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "MabiChatSpeech(ベータ５．１)";
            FormClosing += Main_FormClosing;
            FormClosed += Main_FormClosed;
            Load += Main_Load;
            Shown += Main_Shown;
            KeyDown += Main_KeyDown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox Txt_Chat;
        private System.Windows.Forms.ToolStripStatusLabel SLB_Client;
        private System.Windows.Forms.ToolStripStatusLabel SLB_Ip;
        private System.Windows.Forms.Timer Tim_Status;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Btn_ChatList;
        private System.Windows.Forms.ToolStripButton Btn_Clear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox Cmb_User;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox Cmb_Npc;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripStatusLabel SLB_SaveMode;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox Cmb_Whitelist;
        private System.Windows.Forms.ToolStripButton Btn_List;
        private System.Windows.Forms.ToolStripButton Btn_Setup;
        private System.Windows.Forms.ToolStripButton Btn_DumpView;
        private System.Windows.Forms.ToolStripDropDownButton BTN_SendTask;
        private System.Windows.Forms.ToolStripStatusLabel Btn_Redirect;
        private System.Windows.Forms.ToolStripStatusLabel SLB_Mode;
    }
}

