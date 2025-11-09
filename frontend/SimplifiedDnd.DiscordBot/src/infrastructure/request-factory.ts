import { Character } from '../abstractions/dnd/dnd-service';

export class DndApiRequestFactory {
  public buildCreateCharacterRequest(character: Character): string {
    return `{
  "name": "${character.name}",
  "player_name": "${character.playerId}",
  "specie_name": "${character.specieName}",
  "classes": [
    {
      "name": "${character.className}",
      "level": 1
    }
  ]
}`;
  }
}
