import { Events, Client } from 'discord.js';
import { BaseEventHandler } from '../abstractions/event-handler';

export default class ClientReadyHandler extends BaseEventHandler<Events.ClientReady> {
  constructor() {
    super(true, Events.ClientReady);
  }

  protected handle(readyClient: Client<true>) {
    console.log(`Ready! Logged in as ${readyClient.user.tag}`);
  }
}
