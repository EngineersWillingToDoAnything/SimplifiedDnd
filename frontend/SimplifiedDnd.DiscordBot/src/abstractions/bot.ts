import fs from 'node:fs';
import { Client, Collection, GatewayIntentBits } from 'discord.js';

import { EventHandler } from './event-handler';
import Logger from './logger';
import CommandHandler from './command-handler';

export default class Bot extends Client {
  eventsHandler: Collection<string, EventHandler>;
  commandsHandler: Collection<string, CommandHandler>;
  logger: Logger;

  constructor() {
    super({ intents: [GatewayIntentBits.Guilds] });
    this.eventsHandler = new Collection<string, EventHandler>();
    this.commandsHandler = new Collection<string, CommandHandler>();
    this.logger = new Logger();
  }

  async load(): Promise<void> {
    await this.loadEvents();
    await this.loadCommands();
  }

  async loadEvents(): Promise<void> {
    this.logger.logDivider('Events');

    const eventsFiles = fs.readdirSync('src/events/')
      .filter((file) => file.endsWith('.ts'))
      .map((file) => file.replace('.ts', ''));

    if (eventsFiles.length === 0) {
      return this.logger.logError('No events to load');
    }

    for (const eventFileName of eventsFiles) {
      const EventClass = (await import(`../events/${eventFileName}.ts`)).default;
      const event: EventHandler = new EventClass(this);
      event.startListener(this);
      this.eventsHandler.set(eventFileName, event);
      this.logger.logSuccess(`${eventFileName} loaded`);
    }
  }

  async loadCommands(): Promise<void> {
    this.logger.logDivider('Commands');

    const commandsFiles = fs.readdirSync('src/commands/')
      .filter((file) => file.endsWith('.ts'))
      .map((file) => file.replace('.ts', ''));

    if (commandsFiles.length === 0) {
      return this.logger.logError('No commands to load');
    }

    for (const commandFileName of commandsFiles) {
      const CommandClass = (await import(`../commands/${commandFileName}.ts`)).default;
      const command: CommandHandler = new CommandClass();
      const trimmedCommandName = commandFileName.replace('-handler', '');
      this.commandsHandler.set(trimmedCommandName, command);
      this.logger.logSuccess(`${trimmedCommandName} loaded`);
    }
  }
}