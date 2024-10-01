SELECT
  TOP(20) "dbo"."tbl_t_firewall_cpu"."id" AS "id",
  "dbo"."tbl_t_firewall_cpu"."fk_m_firewall" AS "fk_m_firewall",
  "dbo"."tbl_t_firewall_cpu"."fk_m_run_token" AS "fk_m_run_token",
  "dbo"."tbl_t_firewall_cpu"."fw_cpu_usage_percentage" AS "fw_cpu_usage_percentage",
  "dbo"."tbl_t_firewall_cpu"."created_at" AS "created_at",
  "dbo"."tbl_t_firewall_cpu"."deleted_at" AS "deleted_at",
  "dbo"."tbl_t_firewall_cpu"."fw_cpu_idle_time_percentage" AS "fw_cpu_idle_time_percentage"
FROM
  "dbo"."tbl_t_firewall_cpu"
ORDER BY
  "dbo"."tbl_t_firewall_cpu"."created_at" DESC