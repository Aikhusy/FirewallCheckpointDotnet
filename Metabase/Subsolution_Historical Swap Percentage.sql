SELECT
  TOP(20) "dbo"."tbl_t_firewall_memory"."id" AS "id",
  "dbo"."tbl_t_firewall_memory"."fk_m_firewall" AS "fk_m_firewall",
  "dbo"."tbl_t_firewall_memory"."fk_m_run_token" AS "fk_m_run_token",
  "dbo"."tbl_t_firewall_memory"."mem_type" AS "mem_type",
  "dbo"."tbl_t_firewall_memory"."mem_total" AS "mem_total",
  "dbo"."tbl_t_firewall_memory"."mem_used" AS "mem_used",
  "dbo"."tbl_t_firewall_memory"."mem_free" AS "mem_free",
  "dbo"."tbl_t_firewall_memory"."mem_shared" AS "mem_shared",
  "dbo"."tbl_t_firewall_memory"."mem_cache" AS "mem_cache",
  "dbo"."tbl_t_firewall_memory"."mem_available" AS "mem_available",
  "dbo"."tbl_t_firewall_memory"."created_at" AS "created_at",
  "dbo"."tbl_t_firewall_memory"."deleted_at" AS "deleted_at"
FROM
  "dbo"."tbl_t_firewall_memory"
WHERE
  "dbo"."tbl_t_firewall_memory"."mem_type" = 'Swap'
ORDER BY
  "dbo"."tbl_t_firewall_memory"."created_at" DESC