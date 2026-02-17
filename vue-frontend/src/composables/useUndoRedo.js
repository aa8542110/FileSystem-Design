import { ref, computed } from 'vue'

export function useUndoRedo() {
  const undoStack = ref([])
  const redoStack = ref([])

  const canUndo = computed(() => undoStack.value.length > 0)
  const canRedo = computed(() => redoStack.value.length > 0)

  const undoLabel = computed(() => {
    if (!canUndo.value) return ''
    return undoStack.value[undoStack.value.length - 1].label
  })

  const redoLabel = computed(() => {
    if (!canRedo.value) return ''
    return redoStack.value[redoStack.value.length - 1].label
  })

  const executeCommand = async (command) => {
    await command.execute()
    undoStack.value.push(command)
    redoStack.value = []
  }

  const undo = async () => {
    if (!canUndo.value) return
    const command = undoStack.value.pop()
    await command.undo()
    redoStack.value.push(command)
  }

  const redo = async () => {
    if (!canRedo.value) return
    const command = redoStack.value.pop()
    await command.execute()
    undoStack.value.push(command)
  }

  return {
    canUndo,
    canRedo,
    undoLabel,
    redoLabel,
    executeCommand,
    undo,
    redo
  }
}
