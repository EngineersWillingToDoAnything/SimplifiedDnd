import { Client, ClientEvents } from 'discord.js';

export interface EventHandler {
  startListener(client: Client): void;
}

export abstract class BaseEventHandler<Event extends keyof ClientEvents>
  implements EventHandler
{
  constructor(
    private readonly once: boolean,
    private readonly event: Event,
  ) {}

  public startListener(client: Client): void {
    if (this.once) {
      client.once(this.event, this.handle);
    } else {
      client.on(this.event, this.handle);
    }
  }

  protected abstract handle(...args: ClientEvents[Event]): void | Promise<void>;
}
