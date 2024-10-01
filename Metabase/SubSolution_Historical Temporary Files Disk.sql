SELECT
  TOP(20) "dbo"."tbl_t_firewall_diskspace"."id" AS "id",
  "dbo"."tbl_t_firewall_diskspace"."fk_m_firewall" AS "fk_m_firewall",
  "dbo"."tbl_t_firewall_diskspace"."fk_m_run_token" AS "fk_m_run_token",
  "dbo"."tbl_t_firewall_diskspace"."fw_filesystem" AS "fw_filesystem",
  "dbo"."tbl_t_firewall_diskspace"."fw_mounted_on" AS "fw_mounted_on",
  "dbo"."tbl_t_firewall_diskspace"."fw_total" AS "fw_total",
  "dbo"."tbl_t_firewall_diskspace"."fw_available" AS "fw_available",
  "dbo"."tbl_t_firewall_diskspace"."fw_used" AS "fw_used",
  "dbo"."tbl_t_firewall_diskspace"."fw_used_percentage" AS "fw_used_percentage",
  "dbo"."tbl_t_firewall_diskspace"."created_at" AS "created_at",
  "dbo"."tbl_t_firewall_diskspace"."deleted_at" AS "deleted_at"
FROM
  "dbo"."tbl_t_firewall_diskspace"
WHERE
  (
    (
      "dbo"."tbl_t_firewall_diskspace"."fw_mounted_on" = '/fwtmp'
    )
   
    OR (
      "dbo"."tbl_t_firewall_diskspace"."fw_mounted_on" = '/'
    )
  )
 
ORDER BY
  "dbo"."tbl_t_firewall_diskspace"."created_at" DESC