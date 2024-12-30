
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom"
import { GetRoleWithoutClaims, GetUserList, UpdateRoleUsers } from "./services/role";
import { Role } from "./types";
import { Button, Fieldset, Group, Stack, Title } from "@mantine/core";
import { aggregateData } from "~/shared";
import { CheckboxDataFormat } from "~/components/checkbox-group/types";
import { CheckboxWithGroup } from "~/components/checkbox-group/CheckboxWithGroup";
import { randomId } from "@mantine/hooks";
import { UserInfo } from "~/types";


export const RoleUsers = () => {
  const [role, setRole] = useState<Role>({})
  const [userList, setUserList] = useState<Record<string, CheckboxDataFormat[]>>({})
  const { name: roleName } = useParams();
  const [checkedUsers, setCheckedUsers] = useState<CheckboxDataFormat[]>([])

  useEffect(() => {
    if (roleName) {
      GetRoleWithoutClaims(roleName, (data) => {
        setRole(data as Role)
      })
    }

  }, [roleName])


  useEffect(() => {
    GetUserList(data => {
      if (data && role.users) {
        const groupedData: Record<string, CheckboxDataFormat[]> = aggregateData(data.map(d => ({
          // ...d,
          group: d.profiles?.departmentName,
          label: d.profiles?.fullName,
          value: d.id,
          key: d.id || randomId,
          checked: role.users && role.users.map(ru => ru.id).includes(d.id) || false
        })), 'group') as Record<string, CheckboxDataFormat[]>
        setUserList(groupedData)
      }
    })
  }, [role.users])

  const handleSave = () => {
    if (roleName) {      
      const roleUsers = checkedUsers.length && checkedUsers.map(u => ({ id: u.value }))
      UpdateRoleUsers(roleName, roleUsers as UserInfo[], () => {

      })
    }
  }





  return (<Fieldset legend={<Title order={3}>{role.title}角色-用户分配</Title>}>
    {
      <Stack>
        <div style={{ marginTop: 30 }}>
          <CheckboxWithGroup data={userList} onChange={(values: any) => {            
            setCheckedUsers(values)
          }} />
        </div>
        <Group justify="flex-end" wrap="nowrap">
          <Button mt={60} w={200} onClick={() => {            
            handleSave()
          }} >保存</Button>
        </Group>
      </Stack>

    }
  </Fieldset>)
}