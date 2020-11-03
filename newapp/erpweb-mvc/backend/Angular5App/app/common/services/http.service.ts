
import {Observable } from 'rxjs/Observable';
import {throwError} from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { HttpHeaders, HttpParams } from '@angular/common/http';
import { BlockUIService } from './block-ui.service';

import { HttpErrorResponse } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';



@Injectable()
export class HttpService {

    constructor(private http: HttpClient, private blockUIService: BlockUIService) {
    }

    public post<T>(url: string, object: any): Observable<T> {

        this.blockUIService.startBlock();

        /*if (typeof (Storage) !== 'undefined') {

            let token = localStorage.getItem("CodeProject.Angular4.Token");
            headers.append('Authorization', token);
        }*/


        return this.http.post<T>(url, object, {headers: this.BuildHeaders()}).pipe(
          tap(
            obj => this.blockUIService.stopBlock(),
            obj => this.blockUIService.stopBlock()
          )
        );

    }


    public postNoBlock<T>(url: string, object: any): Observable<T> {

      return this.http.post<T>(url, object, {headers: this.BuildHeaders()});
    }

    public get<T>(url: string, options?: any): Observable<any> {


      this.blockUIService.startBlock();

      if (options == null) {
          options = {};
      }


      options.headers = this.BuildHeaders();

      return this.http.get<T>(url, options).pipe(
        tap(
          obj => this.blockUIService.stopBlock(),
          obj => this.blockUIService.stopBlock()
        ));
    }

    public getNoBlock<T>(url: string, options?: any): Observable<any> {
        if (options == null) {
            options = {};
        }

        options.headers = this.BuildHeaders();
        return this.http.get<T>(url, options);
    }

    public put<T>(url: string, object: any): Observable<T> {

        this.blockUIService.startBlock();

        return this.http.put<T>(url, object, {headers: this.BuildHeaders()}).pipe(
          tap(
            obj => this.blockUIService.stopBlock(),
            obj => this.blockUIService.stopBlock()
          )
        );
    }

    public putNoBlock<T>(url: string, object: any): Observable<T> {

        return this.http.put<T>(url, object, {headers: this.BuildHeaders()});
    }


    private handleError(error: any, blockUIService: BlockUIService, blocking: Boolean) {

        const body = error.json();

        if (blocking) {
            blockUIService.blockUIEvent.emit({
                value: false
            });
        }

        return throwError(body);

    }

    private parseResponse(response: Response, blockUIService: BlockUIService, blocking: Boolean) {

        const authorizationToken = response.headers.get('Authorization');
        if (authorizationToken != null) {

            if (typeof (Storage) !== 'undefined') {
                localStorage.setItem('pointprojects_token', authorizationToken);
            }
        }

        if (blocking) {
            blockUIService.blockUIEvent.emit({
                value: false
            });
        }

        const body = response.json();

        return body;
    }

    private BuildHeaders(): HttpHeaders {
      let headers = new HttpHeaders();
      headers = headers.append('Content-Type', 'application/json; charset=utf-8');
      headers = headers.append('Accept', 'q=0.8;application/json;q=0.9');

      return headers;
    }

    public delete<T>(url: string, options?: any) {

        if (options == null) {
            options = {};
        }
        options.headers = this.BuildHeaders();
        return this.http.delete<T>(url, options);
    }


}
