SELECT
dbo.tbl_m_firewall.fw_name as name,
 "dbo"."tbl_t_firewall_current_status"."id" AS "id",
  "dbo"."tbl_t_firewall_current_status"."fk_m_firewall" AS "fk_m_firewall",
  "dbo"."tbl_t_firewall_current_status"."fk_m_run_token" AS "fk_m_run_token",
  "dbo"."tbl_t_firewall_current_status"."uptime" AS "uptime",
  "dbo"."tbl_t_firewall_current_status"."fwtmp" AS "fwtmp",
  "dbo"."tbl_t_firewall_current_status"."varloglog" AS "varloglog",
  "dbo"."tbl_t_firewall_current_status"."ram" AS "ram",
  "dbo"."tbl_t_firewall_current_status"."swap" AS "swap",
  "dbo"."tbl_t_firewall_current_status"."memory_error" AS "memory_error",
  "dbo"."tbl_t_firewall_current_status"."cpu" AS "cpu",
  "dbo"."tbl_t_firewall_current_status"."rx_error_total" AS "rx_error_total",
  "dbo"."tbl_t_firewall_current_status"."tx_error_total" AS "tx_error_total",
  "dbo"."tbl_t_firewall_current_status"."sync_mode" AS "sync_mode",
  "dbo"."tbl_t_firewall_current_status"."sync_state" AS "sync_state",
  "dbo"."tbl_t_firewall_current_status"."license_expiration_status" AS "license_expiration_status",
  "dbo"."tbl_t_firewall_current_status"."raid_state" AS "raid_state",
  "dbo"."tbl_t_firewall_current_status"."hotfix_module" AS "hotfix_module",
  "dbo"."tbl_t_firewall_current_status"."created_at" AS "created_at"

FROM
  "dbo"."tbl_t_firewall_current_status"
  inner join "dbo".tbl_m_firewall on
  dbo.tbl_t_firewall_current_status.fk_m_firewall = dbo.tbl_m_firewall.id
  