import DndService, { Character } from '../abstractions/dnd/dnd-service';
import Logger from '../abstractions/logger';
import { DndApiRequestFactory } from './request-factory';

enum DndApiEndpoints {
  CreateCharacter = 'api/character',
}

export default class DndApiService implements DndService {
  url: URL;
  requestFactory: DndApiRequestFactory;
  logger: Logger;

  constructor() {
    // TODO: find a better way to handle self-signed certificates
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
    this.url = new URL(
      (process.env.NODE_ENV === 'development'
        ? process.env.services__api__http__0
        : process.env.services__api__https__0) ?? '',
    );
    this.requestFactory = new DndApiRequestFactory();
    this.logger = new Logger('DndApiService');
  }

  async createCharacter(character: Character): Promise<string> {
    const requestBody =
      this.requestFactory.buildCreateCharacterRequest(character);
    const url = new URL(DndApiEndpoints.CreateCharacter, this.url);
    try {
      const response = await fetch(url, {
        body: requestBody,
        headers: {
          'Content-Type': 'application/json',
        },
        method: 'POST',
      });

      return await response.text();
    } catch (error) {
      this.logger.logError(`Failed to create character: ${error}`);
      return '';
    }
  }
}
