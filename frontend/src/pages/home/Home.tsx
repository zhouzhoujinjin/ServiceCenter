import { ActionIcon, Box, Divider, Flex, ScrollArea, Stack, TextInput, Title } from "@mantine/core"
import { IconQuestionMark } from "@tabler/icons-react"
import { useState } from "react"
import Markdown from 'marked-react';
import { Prompt } from "./services/ai"
import styles from './Home.module.css'

export const Home = () => {

  const [question, setQuestion] = useState<string>("")
  const [answer, setAnswer] = useState<string>("")
  const [count, setCount] = useState(1)
  const [loading, setLoading] = useState(false)
  const Ask = () => {
    setLoading(true)
    Prompt(question, (rsp) => {
      console.log(rsp)
      if (rsp) {
        setAnswer(a => (`${a}\n\n#### ${count}. 问题: ${question}\n\n#### 回答:\n\n${rsp.response}`))
        setCount(c => c + 1)
        setLoading(false)
      }
    })
  }




  return (
    <>
      <Flex align='start' gap='lg'>
        <Stack>
          <Title order={1}>写下你想问的，我来帮你</Title>
          <TextInput w='600'
            rightSection={
              <ActionIcon loading={loading} onClick={() => {
                Ask()
              }} color="yellow" >
                <IconQuestionMark />
              </ActionIcon>
            }
            onChange={e => {
              setQuestion(e.target.value)
            }} />
        </Stack>
        <Divider orientation="vertical" />
        <Box className={styles.answer}>
          <Title order={1}>我的回答</Title>
          <ScrollArea.Autosize type="scroll" mah={500}>
            <Markdown>
              {answer}
            </Markdown>
          </ScrollArea.Autosize>
        </Box>
      </Flex>
    </>
  )
}