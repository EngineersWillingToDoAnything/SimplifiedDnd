export interface Character {
  name: string
  playerId: string
  specieName: string
  className: string
}

export default interface DndService {
  createCharacter(character: Character): Promise<string>;
// eslint-disable-next-line semi
}