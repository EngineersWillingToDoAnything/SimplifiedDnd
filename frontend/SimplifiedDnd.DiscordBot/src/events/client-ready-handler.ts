import { Events, Client } from 'discord.js';
import { BaseEventHandler } from '../abstractions/event-handler';
import Logger from '../abstractions/logger';

export default class ClientReadyHandler extends BaseEventHandler<Events.ClientReady> {
  readonly logger: Logger;

  constructor() {
    super(true, Events.ClientReady);
    this.logger = new Logger('ClientReadyHandler');
  }

  protected readonly handle = (readyClient: Client<true>) => {
    this.logger.logInfo(`Ready! Logged in as ${readyClient.user.tag}`);
  };
}
