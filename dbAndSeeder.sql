USE master;
GO

DROP DATABASE IF EXISTS DBO_CHECKPOINT_FIREWALL;
GO

CREATE DATABASE DBO_CHECKPOINT_FIREWALL;
GO

USE DBO_CHECKPOINT_FIREWALL;
GO

-- Tabel master untuk data firewall
CREATE TABLE tbl_m_firewall (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fw_name VARCHAR(30),
	fw_location VARCHAR(30),
	fw_site VARCHAR(30),
	fk_fw_pair BIGINT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel master untuk run token
CREATE TABLE tbl_m_run_token (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	run_token VARCHAR(30),
	run_type VARCHAR(20),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel referensi untuk login firewall
CREATE TABLE tbl_r_firewall_login (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fw_ip_address VARCHAR(255),
	fw_username VARCHAR(255),
	fw_password VARCHAR(255),
	fw_expert_password VARCHAR(255),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk data uptime firewall
CREATE TABLE tbl_t_firewall_uptime (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_current_time TIME,
	fw_days_uptime INT,
	fw_uptime TIME,
	fw_number_of_users INT,
	fw_load_avg_1_min FLOAT,
	fw_load_avg_5_min FLOAT,
	fw_load_avg_15_min FLOAT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk disk space firewall
CREATE TABLE tbl_t_firewall_diskspace (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_filesystem VARCHAR(40),
	fw_mounted_on VARCHAR(20),
	fw_total INT,
	fw_available INT,
	fw_used INT,
	fw_used_percentage INT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk memory firewall
CREATE TABLE tbl_t_firewall_memory (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	mem_type VARCHAR(30),
	mem_total INT,
	mem_used INT,
	mem_free INT,
	mem_shared INT,
	mem_cache INT,
	mem_available INT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk CPU firewall
CREATE TABLE tbl_t_firewall_cpu (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_cpu_user_time_percentage INT,
	fw_cpu_system_time_percentage INT,
	fw_cpu_idle_time_percentage INT,
	fw_cpu_usage_percentage INT,
	fw_cpu_queue_length INT,
	fw_cpu_interrupt_per_sec INT,
	fw_cpu_number INT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel referensi untuk lisensi firewall
CREATE TABLE tbl_t_firewall_license (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_license_host VARCHAR(30),
	fw_license_expiration VARCHAR(30),
	fw_license_feature VARCHAR(max),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel referensi untuk kontrak firewall
CREATE TABLE tbl_t_firewall_contract (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_contract_expiration VARCHAR(30),
	fw_contract_total int,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);


-- Tabel transaksi untuk status firewall
CREATE TABLE tbl_t_firewall_current_status (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	uptime VARCHAR(50),
	fwtmp INT,
	varloglog INT,
	ram FLOAT,
	swap FLOAT,
	memory_error FLOAT,
	cpu INT,
	rx_error_total INT,
	tx_error_total INT,
	sync_mode VARCHAR(30),
	sync_state VARCHAR(30),
	license_expiration_status VARCHAR(200),
	raid_state VARCHAR(30),
	hotfix_module VARCHAR(200),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk RAID firewall
CREATE TABLE tbl_t_firewall_raid (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	raid_volume_id INT,
	raid_level VARCHAR(40),
	raid_number_of_disks INT,
	raid_size VARCHAR(40),
	raid_state VARCHAR(40),
	raid_flag VARCHAR(40),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk RX/TX firewall
CREATE TABLE tbl_t_firewall_rxtx (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	interface VARCHAR(50) NOT NULL,
	hwaddr VARCHAR(17) NOT NULL,
	inet_addr VARCHAR(15),
	bcast VARCHAR(15),
	mask VARCHAR(15),
	mtu INT,
	metric INT,
	rx_packets BIGINT,
	rx_errors BIGINT,
	rx_dropped BIGINT,
	rx_overruns BIGINT,
	rx_frame BIGINT,
	tx_packets BIGINT,
	tx_errors BIGINT,
	tx_dropped BIGINT,
	tx_overruns BIGINT,
	tx_carrier BIGINT,
	collisions BIGINT,
	txqueuelen BIGINT,
	rx_bytes BIGINT,
	rx_human_readable VARCHAR(20),
	tx_bytes BIGINT,
	tx_human_readable VARCHAR(20),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel referensi untuk hotfix firewall
CREATE TABLE tbl_t_firewall_hotfix (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_kernel VARCHAR(30),
	fw_build_number VARCHAR(30),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk failed memory firewall
CREATE TABLE tbl_t_firewall_failed_memory (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_total_memory INT,
	fw_peak_memory INT,
	fw_total_alloc INT,
	fw_failed_alloc INT,
	fw_total_free INT,
	fw_failed_free INT,
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk capacity optimization firewall
CREATE TABLE tbl_t_firewall_capacity_optimisation (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_hostname VARCHAR(30),
	fw_names VARCHAR(30),
	fw_id INT,
	fw_vals INT,
	fw_peaks INT,
	fw_slinks INT,
	fw_limit VARCHAR(30),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);

-- Tabel transaksi untuk ClusterXL firewall
CREATE TABLE tbl_t_firewall_clusterxl (
	id BIGINT NOT NULL IDENTITY(1,1),
	PRIMARY KEY (id),
	fk_m_firewall BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_firewall(id),
	fk_m_run_token BIGINT NOT NULL FOREIGN KEY REFERENCES tbl_m_run_token(id),
	fw_sync_mode VARCHAR(30),
	fw_ip_address VARCHAR(30),
	fw_load VARCHAR(30),
	fw_state VARCHAR(30),
	created_at DATETIME DEFAULT GETDATE(),
	deleted_at DATETIME
);


-- Insert default run token
INSERT INTO tbl_m_run_token(run_token, run_type) VALUES('current status', 'current status');

insert into tbl_m_firewall (fw_name, fw_location, fw_site ) values ( 'FW-JIEP03','jakarta','HO' ), ('CILEFWNS622','jakarta','HO' );

INSERT INTO tbl_r_firewall_login (fk_m_firewall, fw_ip_address, fw_username, fw_password, fw_expert_password) 
VALUES 
(1, '172.24.39.244', 'pkl_2024', 'efmlUB5bEt/phHJ/CerJkJcEBxGS/fEBZgq9gO+6Ao7QSkHtkxu/Y12ZItxnM8ZwFJVf/g==', 'BJhSOnGtr9xWuVmDcd0v7juTRUo2cJ79kMPfiYu50HVSZNQ8Y3QKijttdDbq6WvsxGFbiJg='),
(2, '172.24.39.245', 'pkl_2024', 'koo0A2kkv0lDuVzCJtlf/q2gBu1L2RIjAJ75Qo3BNzYuOi/6QwgSCdSvqbRjt0/8UFpfHA==', '9BwFmjvGKc3H8m7lgqmjXXLG0m2F54xpvfKsAKF4j7TOoY1UqdMQcPvoLj1kvAuaIpgJU2U=');

