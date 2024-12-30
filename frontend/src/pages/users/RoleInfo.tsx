import { useEffect, useState } from "react"
import { CreateRole, GetPermissions, GetRole, UpdateRole } from "./services/role"
import { Permission } from "./types"
import { TransferList } from "~/components/transfer-list/TransferList"
import { useParams } from "react-router-dom"
import { Button, Fieldset, Group, Input, Stack } from "@mantine/core"


export const RoleInfo = () => {
  const [permissions, setPermissions] = useState<Permission[]>([])
  const [target, setTarget] = useState<Permission[]>([])
  const [selectedPermissions, setSelectedPermissions] = useState<Permission[]>([])
  const [title, setTitle] = useState<string>('')
  const [name, setName] = useState<string>('')
  const role = useParams();

  useEffect(() => {
    GetPermissions((data) => {
      if (data) {
        setPermissions(data)
      }
    })
  }, [])

  useEffect(() => {
    if (role.name && role.name !== 'create') {
      GetRole(role.name as string, (data) => {
        if (data) {
          setTitle(data.title || "")
          setName(data.name || role.name || "")
        }
        if (permissions && permissions.length > 0) {
          const target: Permission[] = [];
          if (data && data?.claims?.length) {

            data.claims.forEach(d => {
              const permission = permissions.filter(p => p.value === d);
              target.push(...permission);
            });
            console.log(data)
            setTitle(data.title as string);
            setName(data.name as string)
            setTarget(target);
            setSelectedPermissions(target)
          }
        }
      })
    }
  }, [permissions, role])


  const getClaims = (permissions: Permission[]) => {
    if (permissions.length > 0) {
      const claims = permissions.map(p => {
        const o = p.value
        if (o.slice(0, 1) === "/") {
          return `route,${o}`;
        } else if (
          o.startsWith("GET") ||
          o.startsWith("PUT") ||
          o.startsWith("POST") ||
          o.startsWith("DELETE")
        ) {
          return `api,${o}`;
        } else {
          return `action,${o}`;
        }
      })
      return claims;
    }
  }

  const handleSubmit = () => {
    if (role.name === 'create') {
      if (selectedPermissions && title && name) {
        const roleClaims = getClaims(selectedPermissions);
        CreateRole({ name: name, title: title, claims: roleClaims })
      }
    } else {
      if (selectedPermissions && title && name) {
        const roleClaims = getClaims(selectedPermissions);
        UpdateRole(name, { name: name, title: title, claims: roleClaims })
      }
    }
  }


  return (
    permissions &&
    <Stack>
      <Group justify="flex-start" wrap="nowrap">
        <Input.Wrapper label="角色标题" description="中文名称" withAsterisk>
          <Input placeholder="输入标题" value={title} onChange={(e) => setTitle(e.target.value)} />
        </Input.Wrapper>
        <Input.Wrapper label="角色名称" description="英文名称" withAsterisk>
          <Input placeholder="输入标题" value={name} onChange={(e) => setName(e.target.value)} disabled={role.name !== 'create'} />
        </Input.Wrapper>
      </Group>
      <Fieldset legend="权限分配">
        <TransferList
          source={permissions}
          target={target}
          onChange={(v) => {
            setSelectedPermissions(v)
          }}
        />
      </Fieldset>
      <Group justify="flex-end" wrap="nowrap">
        <Button mt={60} w={200} onClick={() => {
          handleSubmit();
        }} >保存</Button>
      </Group>
    </Stack>
  )
}