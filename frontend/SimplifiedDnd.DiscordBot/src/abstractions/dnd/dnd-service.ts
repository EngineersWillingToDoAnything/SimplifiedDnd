import { Maybe } from '../maybe-types';

export interface Character {
  name: string;
  playerId: string;
  specieName: string;
  className: string;
}

export default interface DndService {
  createCharacter(character: Character): Promise<Maybe<string>>;
}
