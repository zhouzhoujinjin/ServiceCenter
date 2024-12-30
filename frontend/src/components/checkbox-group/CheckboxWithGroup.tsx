import { useListState } from '@mantine/hooks';
import { Checkbox } from '@mantine/core';
import { FC, useEffect, useState } from 'react';
import { CheckboxDataFormat } from './types';




const IndeterminateCheckbox = ({ data, groupLabel, onChange }: {
  data: any[], groupLabel: string,
  onChange?: (values: CheckboxDataFormat[]) => void
}) => {
  const [values, handlers] = useListState(data);
  const allChecked = values.every((value) => value.checked);
  const indeterminate = values.some((value) => value.checked) && !allChecked;

  useEffect(() => {    
    onChange && onChange(values)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [values])

  const items = values.map((value, index) => (
    <Checkbox
      mt={6}
      ml={33}
      label={value.label}
      key={value.key}
      checked={value.checked}
      onChange={(event) => {
        handlers.setItemProp(index, 'checked', event.currentTarget.checked)
      }}
    />
  ));

  return (
    <>
      <Checkbox
        mt={9}
        checked={allChecked}
        indeterminate={indeterminate}
        label={groupLabel}
        onChange={() => {
          handlers.setState((current) => current.map((value) => ({ ...value, checked: !allChecked })))
        }}
      />
      {items}
    </>
  );
}

export const CheckboxWithGroup: FC<{
  data: Record<string, CheckboxDataFormat[]>
  onChange?: (data: any) => void
}>
  = ({ data, onChange }) => {
    const [merge, setMerge] = useState<CheckboxDataFormat[]>([])
    const keys = Object.keys(data);

    useEffect(() => {
      const init: CheckboxDataFormat[] = [];
      keys.forEach(k => {
        data[k].filter(d => d.checked).forEach(dd => init.push(dd))
      })      
      setMerge(init)
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [data])

    useEffect(() => {
      onChange && onChange(merge)
    }, [onChange, merge])



    return <>
      {keys.map((k) => (<IndeterminateCheckbox key={k}
        data={data[k]}
        groupLabel={k}
        onChange={(v: CheckboxDataFormat[]) => {
          const add = v.filter(v => v.checked && !merge.map(m => m.key).includes(v.key));
          const remove = v.filter(v => !v.checked)
          const newMerge = [...merge, ...add].filter(n => !remove.map(u => u.key).includes(n.key))
          setMerge(newMerge)
        }}
      />))}</>
  }