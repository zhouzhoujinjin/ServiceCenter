import { useListState, randomId } from '@mantine/hooks';
import { Checkbox } from '@mantine/core';
import { useEffect, useState } from 'react';

// const initialValues = [
//   { label: 'Receive email notifications', checked: false, key: randomId(), group: '测试' },
//   { label: 'Receive sms notifications', checked: false, key: randomId(), group: '测试' },
//   { label: 'Receive push notifications', checked: false, key: randomId(), group: '实际' },
// ];

const initialValues2: Record<string, any[]> = {
  "测试": [
    { label: 'Receive email notifications', checked: false, key: randomId(), group: '测试' },
    { label: 'Receive sms notifications', checked: false, key: randomId(), group: '测试' },
    { label: 'Receive push notifications', checked: false, key: randomId(), group: '实际' },
  ],
  "实际": [
    { label: 'Receive push notifications', checked: false, key: randomId(), group: '实际' },
  ]
}






const IndeterminateCheckbox = ({ data, groupLabel, onChange }: { data: any[], groupLabel: string, onChange?: (values: string[], checked: boolean) => void }) => {
  const [values, handlers] = useListState(data);
  const allChecked = values.every((value) => value.checked);
  const indeterminate = values.some((value) => value.checked) && !allChecked;

  const items = values.map((value, index) => (
    <Checkbox
      mt="xs"
      ml={33}
      label={value.label}
      key={value.key}
      checked={value.checked}
      onChange={(event) => {
        handlers.setItemProp(index, 'checked', event.currentTarget.checked)
        onChange && onChange([value.label], event.currentTarget.checked)
      }}
    />
  ));

  return (
    <>
      <Checkbox
        checked={allChecked}
        indeterminate={indeterminate}
        label={groupLabel}
        onChange={() => {
          handlers.setState((current) => {
            const n = current.map((value) => ({ ...value, checked: !allChecked }))
            onChange && onChange(n.filter(v => v.checked).map(v => v.label), !allChecked)
            return n;
          }
          )
        }}
      />
      {items}
    </>
  );
}

export const CheckboxWithGroup_slobber = () => {

  const [selectedItems, setSelectedItems] = useState<Record<string, string[]>>({})

  useEffect(() => {
    console.log(selectedItems)
  }, [selectedItems])

  const keys = Object.keys(initialValues2);
  return <>
    {keys.map((k) => (<IndeterminateCheckbox key={k}
      data={initialValues2[k]}
      groupLabel={k}
      onChange={(values: string[], checked: boolean) => {
        setSelectedItems(v => {
          if (checked) {
            return { ...v, [k]: [...(v[k] || []), ...values] }
          } else {
            values.forEach(val => {
              const i = (v[k] || []).indexOf(val)
              if (i > -1) {
                v[k].splice(i, 1)
              }
            })
            return { ...v, [k]: [...(v[k] || [])] }
          }
        })
      }}
    />))}</>


}