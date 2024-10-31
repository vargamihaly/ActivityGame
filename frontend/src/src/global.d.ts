import type * as vite from 'vite'
import type * as node from 'node'

declare global {
    interface FSWatcher extends vite.FSWatcher, node.FSWatcher {}
}