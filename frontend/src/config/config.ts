import { z } from 'zod'

const configSchema = z.object({
  activityGameApi: z.object({
    endpoint: z.string(),
    timeout: z.number(),
    withCredentials: z.boolean(),
  }),
})

export type Config = z.infer<typeof configSchema>

let CONFIG: Config;

try {
  CONFIG = configSchema.parse({
    activityGameApi: {
      endpoint: '/api', // This will always be correct due to the Vite proxy
      timeout: 40000,
      withCredentials: true,
    },
  })
} catch (error) {
  console.error('Configuration error:', error);
  throw error;
}

export { CONFIG };