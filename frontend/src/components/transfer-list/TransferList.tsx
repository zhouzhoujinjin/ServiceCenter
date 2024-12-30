import { FC, useEffect, useState } from 'react';
import { IconChevronRight } from '@tabler/icons-react';
import { ActionIcon, Checkbox, Combobox, Group, ScrollArea, TextInput, useCombobox } from '@mantine/core';
import classes from './TransferList.module.css';
import { aggregateData } from '~/shared';


interface RenderListProps {
  options: any[];
  onTransfer: (options: string[]) => void;
  type: 'forward' | 'backward';
}

function RenderList({ options, onTransfer, type }: RenderListProps) {
  const combobox = useCombobox();
  const [value, setValue] = useState<string[]>([]);
  const [search, setSearch] = useState('');

  const handleValueSelect = (val: string) => {
    setValue((current) =>
      current.includes(val) ? current.filter((v) => v !== val) : [...current, val]
    );
  }
  const groupedOptions = aggregateData(options, 'group');
  const items = Object.keys(groupedOptions || {})
    .filter((item) => item.includes(search.trim()))
    .map(key => (
      <Combobox.Group label={key} key={key}>
        {
          groupedOptions && groupedOptions[key].map((item: any) => (
            <Combobox.Option
              value={item.value}
              key={item.value}
              active={value.includes(item.value)}
              onMouseOver={() => combobox.resetSelectedOption()}
            >
              <Group gap="sm">
                <Checkbox
                  checked={value.includes(item.value)}
                  onChange={() => { }}
                  aria-hidden
                  tabIndex={-1}
                  style={{ pointerEvents: 'none' }}
                />
                <span>{item.name}</span>
              </Group>
            </Combobox.Option>
          ))
        }
      </Combobox.Group>

    ))

  return (
    <div className={classes.renderList} data-type={type}>

      <Combobox store={combobox} onOptionSubmit={handleValueSelect}>
        <Combobox.EventsTarget>
          <Group wrap="nowrap" gap={0} className={classes.controls}>
            <TextInput w='100%'
              placeholder="Search groceries"
              classNames={{ input: classes.input }}
              value={search}
              onChange={(event) => {
                setSearch(event.currentTarget.value);
                combobox.updateSelectedOptionIndex();
              }}
            />
            <ActionIcon
              radius={0}
              variant="default"
              size={36}
              className={classes.control}
              onClick={() => {
                onTransfer(value);
                setValue([]);
              }}
            >
              <IconChevronRight className={classes.icon} />
            </ActionIcon>
          </Group>
        </Combobox.EventsTarget>
        <ScrollArea.Autosize type="scroll" mah={500}>
          <div className={classes.list}>
            <Combobox.Options>
              {items.length > 0 ? items : <Combobox.Empty>Nothing found....</Combobox.Empty>}
            </Combobox.Options>
          </div>
        </ScrollArea.Autosize>
      </Combobox>

    </div>
  );
}

export const TransferList: FC<{ source: any[], target: any[], onChange?: (values: any[]) => void }> =
  ({ source, target, onChange }) => {
    const [data, setData] = useState<[any[], any[]]>([[], []]);
    useEffect(() => {
      const initSource = source.filter(s => !target.map(t => t.value).includes(s.value))
      setData([[...initSource], [...target]])
    }, [source, target])

    const handleTransfer = (transferFrom: number, options: string[]) => {

      const transferTo = transferFrom === 0 ? 1 : 0;
      const transferFromData = data[transferFrom].filter((item) => !options.includes(item.value));

      const toData: any = [];
      if (options.length) {
        options.forEach(o => {
          toData.push(...data[transferFrom].filter(f => f.value === o))
        })
      }
      const transferToData = [...data[transferTo], ...toData];
      const result: [any[], any[]] = [[], []];
      result[transferFrom] = transferFromData;
      result[transferTo] = transferToData;
      setData(result);
      onChange?.(result[1])
    }

    return (
      <div className={classes.root}>
        <RenderList
          type="forward"
          options={data[0]}
          onTransfer={(options) => {
            handleTransfer(0, options)
          }}
        />
        <RenderList
          type="backward"
          options={data[1]}
          onTransfer={(options) => {
            handleTransfer(1, options)
          }}
        />
      </div>
    );
  }